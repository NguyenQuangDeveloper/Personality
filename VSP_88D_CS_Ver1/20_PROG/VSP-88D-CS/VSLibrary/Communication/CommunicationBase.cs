using CommunityToolkit.Mvvm.ComponentModel;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Communication
{
    public class CommunicationEventArgs : EventArgs
    {
        public string ClassName { get; }
        public CommunicationEventType EventType { get; }
        public string Message { get; }
        public CommunicationEventArgs(string CN, CommunicationEventType type, string message)
        {
            ClassName = CN;
            EventType = type;
            Message = $"[{CN}][{type.ToString()}] : " + message;
        }
    }

    /// <summary>
    /// 공통 로직(이벤트 발생, 상태 관리, 프레임 분리, 요청-응답 매칭, 송신 동기화, 주기 작업)을 구현한 기본 클래스입니다.
    /// </summary>
    [ObservableObject]
    public abstract partial class CommunicationBase : PacketHelper, ICommunication, IDisposable
    {
        // 이벤트: 처리된 패킷을 구독자에게 전달
        public event EventHandler<CommunicationEventArgs>? CommunicationEvent;

        protected bool ShouldAutoInitialize = true;

        //통신 에러 누적관련
        // 설정: 지정 기간(_errorTimeMs) 안에 누적해야 할 에러 호출 횟수
        private readonly int _errorCount = 3;
        // 설정: 에러 누적 기간 (밀리초 단위)
        private readonly int _errorTimeMs = 1000;

        // 내부 상태: 현재 기간에서 집계된 에러 호출 수
        private int _currentCount = 0;
        // 내부 상태: 집계 기간 시작 시점 (UTC 기준)
        private DateTime _periodStartUtc = DateTime.MinValue;

        protected virtual void EventMessage(string CN, CommunicationEventType type, string message)
        {
            CommunicationEvent?.Invoke(this, new CommunicationEventArgs(CN, type, message));
        }

        [ObservableProperty]
        private bool isOpen;

        protected bool _isWorking = false;

        public ICommunicationConfig Config { get; set; } = default!;

        public List<string> MethodList { get; set; } = default!;

        // --- 프레임 분리용 필드 & 메서드 ---
        protected byte[]? _startSeq;
        protected byte[]? _delimiter;
        private const int MAX_BUFFER_SIZE = 4096;
        private readonly List<byte> _recvBuffer = new();

        /// <summary>
        /// 에러 발생 시마다 호출합니다.
        /// _errorTimeMs(ms) 이내에 _errorCount 회 호출되면 OnError 이벤트를 발생시키고 상태를 초기화합니다.
        /// </summary>
        protected void ErrorMonitor()
        {
            var now = DateTime.UtcNow;

            // 첫 호출이거나, 이전 기간이 만료된 경우 새 기간으로 초기화
            if (_currentCount == 0 || (now - _periodStartUtc).TotalMilliseconds > _errorTimeMs)
            {
                _periodStartUtc = now;
                _currentCount = 1;
            }
            else
            {
                // 같은 기간 내 호출이므로 카운트 증가
                _currentCount++;
            }

            // 기준치 도달 시 이벤트 발생 후 초기화
            if (_currentCount >= _errorCount)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, "연속적인 통신에러 점검 필요");
                _currentCount = 0;
                _periodStartUtc = DateTime.MinValue;
            }
        }

        /// <summary>
        /// 버퍼에서 시퀀스의 시작 인덱스를 찾습니다.
        /// </summary>
        protected int IndexOfSequence(List<byte> buffer, byte[] seq)
        {
            for (int i = 0; i + seq.Length <= buffer.Count; i++)
            {
                bool match = true;
                for (int j = 0; j < seq.Length; j++)
                {
                    if (buffer[i + j] != seq[j]) { match = false; break; }
                }
                if (match) return i;
            }
            return -1;
        }

        /// <summary>
        /// 수신된 원시 바이트를 프레임으로 분리하고 처리합니다.
        /// </summary>
        protected void ProcessReceivedBytes(byte[] chunk)
        {
            _recvBuffer.AddRange(chunk);

            while (true)
            {
                // 1) 시작 시퀀스 처리
                if (_startSeq?.Length > 0)
                {
                    int idxStart = IndexOfSequence(_recvBuffer, _startSeq);
                    if (idxStart < 0)
                        break;  // 아직 시작문자가 안 들어왔으면 탈출
                                // 시작문자 앞의 쓰레기 바이트와 시작문자 자체를 제거
                    _recvBuffer.RemoveRange(0, idxStart + _startSeq.Length);
                }

                // 2) 종료(delimiter) 찾기
                if (_delimiter?.Length > 0)
                {
                    int idxDelim = IndexOfSequence(_recvBuffer, _delimiter);
                    if (idxDelim < 0)
                        break;  // 아직 끝문자가 안 들어왔으면 탈출

                    // 시작문자를 뗀 뒤에 남은 [0..idxDelim) 구간이 순수 페이로드
                    var packet = _recvBuffer.Take(idxDelim).ToArray();
                    ProcessPacket(packet);
                    //var strdata = Encoding.ASCII.GetBytes($"[{Config.CommunicationName}] RX : ");
                    //var combined = strdata.Concat(packet).ToArray();
                    //OnDataReceived(combined);
                    // 끝문자까지 통째로 제거
                    _recvBuffer.RemoveRange(0, idxDelim + _delimiter.Length);
                }
                else
                {
                    // delimiter 자체를 안 쓰면 무한루프 방지
                    break;
                }
            }

            // 너무 커지면 초기화
            if (_recvBuffer.Count > MAX_BUFFER_SIZE)
                _recvBuffer.Clear();
        }

        // --- 요청-응답 매칭 큐 ---
        private class PendingRequest
        {
            public Func<byte[], bool> Matcher { get; init; } = null!;
            public TaskCompletionSource<byte[]> Tcs { get; init; } = null!;
        }
        private readonly List<PendingRequest> _pendingRequests = new();
        private readonly object _pendingLock = new();

        /// <summary>
        /// 매칭된 요청에 응답을 제공하고, 매칭되지 않으면 OnPacket 호출
        /// </summary>
        private void ProcessPacket(byte[] packet)
        {
            // 1) 요청-응답 매칭
            PendingRequest? matched = null;
            lock (_pendingLock)
            {
                matched = _pendingRequests.FirstOrDefault(r => r.Matcher(packet));
                if (matched != null)
                {
                    _pendingRequests.Remove(matched);
                    matched.Tcs.TrySetResult(packet);
                }
            }

            // 2) 접두사 결정
            string prefix = matched != null
                ? $"[Sync] "
                : $"";

            // 3) 페이로드 조합
            byte[] headerBytes = Encoding.ASCII.GetBytes(prefix);
            byte[] combined = new byte[headerBytes.Length + packet.Length];
            Buffer.BlockCopy(headerBytes, 0, combined, 0, headerBytes.Length);
            Buffer.BlockCopy(packet, 0, combined, headerBytes.Length, packet.Length);

            // 4) 이벤트 호출은 락 밖에서
            try
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.Rx, Encoding.ASCII.GetString(combined));
                //DataReceived?.Invoke(this, combined);
            }
            catch (Exception ex)
            {
                // 이벤트 핸들러 중 예외가 나도 통신 루프가 멈추지 않도록
                EventMessage(Config.CommunicationName, CommunicationEventType.RxError, $"DataReceived handler threw exception : {ex.Message}");
            }

            // 5) 매칭되지 않은 경우만 OnPacket 호출
            if (matched == null)
                OnPacket(packet);            
        }

        /// <summary>
        /// 요청-응답 매칭되지 않은 패킷을 처리하려면 오버라이드
        /// </summary>
        protected virtual void OnPacket(byte[] packet, bool background = true)
        {
        }

        // --- 송신 동기화 ---
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        public virtual async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            await _sendLock.WaitAsync(cancellationToken);
            try
            {
                await WriteCoreAsync(data, cancellationToken);

                // --- 2) TX 로그 발생 --- CR/LF 제거
                var payloadText = Encoding.ASCII.GetString(data).TrimEnd('\r', '\n');
                var payloadBytes = Encoding.ASCII.GetBytes(payloadText);

                var prefix = $"";
                var header = Encoding.ASCII.GetBytes(prefix);
                var combined = new byte[header.Length + payloadBytes.Length];

                Buffer.BlockCopy(header, 0, combined, 0, header.Length);
                Buffer.BlockCopy(payloadBytes, 0, combined, header.Length, payloadBytes.Length);

                EventMessage(Config.CommunicationName, CommunicationEventType.Tx, Encoding.ASCII.GetString(combined));
                //DataReceived?.Invoke(this, combined);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// 실제 쓰기 로직 (SerialBase, SocketBase에서 구현)
        /// </summary>
        protected abstract Task WriteCoreAsync(byte[] data, CancellationToken cancellationToken);

        // --- 요청-응답 송수신 ---
        public virtual async Task<byte[]?> SendReceiveAsync(
            byte[] data,
            Func<byte[], bool> responseMatcher,
            int timeoutMs = 1000)
        {
            // 요청 전송과 응답 대기 전체 구간을 하나의 락으로 보호
            await _sendLock.WaitAsync();
            try
            {
                var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
                var pending = new PendingRequest
                {
                    Matcher = responseMatcher,
                    Tcs = tcs
                };

                lock (_pendingLock)
                {
                    _pendingRequests.Add(pending);
                }

                // --- 1) TX 로그 ---
                {
                    var payloadText = Encoding.ASCII.GetString(data).TrimEnd('\r', '\n');
                    var payloadBytes = Encoding.ASCII.GetBytes(payloadText);

                    var prefix = $"[Sync] ";
                    var header = Encoding.ASCII.GetBytes(prefix);
                    var combined = new byte[header.Length + payloadBytes.Length];

                    Buffer.BlockCopy(header, 0, combined, 0, header.Length);
                    Buffer.BlockCopy(payloadBytes, 0, combined, header.Length, payloadBytes.Length);

                    EventMessage(Config.CommunicationName, CommunicationEventType.Tx, Encoding.ASCII.GetString(combined));
                }

                // 실제 쓰기 (WriteCoreAsync) → 응답 대기
                await WriteCoreAsync(data, CancellationToken.None);

                var timeoutTask = Task.Delay(timeoutMs);
                var completed = await Task.WhenAny(tcs.Task, timeoutTask);

                return completed == tcs.Task ? await tcs.Task : null;
            }
            finally
            {
                // 남아있는 PendingRequest 제거
                lock (_pendingLock)
                {
                    _pendingRequests.RemoveAll(r => !r.Tcs.Task.IsCompleted);
                }
                _sendLock.Release();
            }
        }

        // --- 주기 작업 (필요 시 오버라이드) ---
        protected CancellationTokenSource _doWorkCts;
        protected virtual async Task DoWorkAsync() {}

        public virtual Task InitializeAsync() { return Task.CompletedTask; }

        // --- 추상 및 공통 메서드 ---
        public abstract Task OpenAsync(CancellationToken cancellationToken = default);
        public virtual Task CloseAsync(CancellationToken cancellationToken = default)
        {
            // 주기 작업 취소
            try { _doWorkCts.Cancel(); } catch { }
            return Task.CompletedTask;
        }

        // Dispose 패턴
        public virtual void Dispose()
        {
            try { CloseAsync().Wait(); } catch { }
        }

        public Task OnBgpacketAsync(CancellationToken cancellationToken = default)
        {
            Config.BackgroundPacket = true;
            return Task.CompletedTask;
        }

        public Task OffBgpacketAsync(CancellationToken cancellationToken = default)
        {
            Config.BackgroundPacket = false;
            return Task.CompletedTask;
        }

        public abstract Task OnDoworkAsync(CancellationToken cancellationToken = default);

        public abstract Task OffDoworkAsync(CancellationToken cancellationToken = default);

        public async Task CallMethodAsync(string methodName, params object[] args)
        {
            var target = this;
            if (target == null || string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException();

            var type = target.GetType();

            // 오버로드 포함 모든 public instance 메서드 중 이름 일치 + async 만 필터링
            var candidates = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.Name == methodName)
                .Where(m => typeof(Task).IsAssignableFrom(m.ReturnType)) // async만
                .ToList();

            if (candidates.Count == 0)
                throw new MissingMethodException($"메서드 '{methodName}' 를 찾을 수 없습니다.");

            // 인자 수, 타입 일치하는 메서드 고르기
            MethodInfo? method = candidates.FirstOrDefault(m =>
            {
                var parameters = m.GetParameters();
                if (parameters.Length != args.Length) return false;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var expectedType = parameters[i].ParameterType;
                    var givenArg = args[i];

                    try
                    {
                        if (givenArg == null)
                        {
                            if (expectedType.IsValueType && Nullable.GetUnderlyingType(expectedType) == null)
                                return false; // null 불가한 값형인데 null 전달됨
                        }
                        else
                        {
                            Convert.ChangeType(givenArg, expectedType); // 변환 가능 여부 확인
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            });

            if (method == null)
                throw new MissingMethodException($"'{methodName}' 메서드를 인자 타입과 개수로 찾을 수 없습니다.");

            // 파라미터 변환
            var finalArgs = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(args[i], p.ParameterType))
                .ToArray();

            var result = method.Invoke(target, finalArgs);

            if (result is Task task)
                await task;
            else
                throw new InvalidOperationException("비동기 Task 반환 메서드가 아닙니다.");
        }
    }

    /// <summary>
    /// 공통 CRC/LRC/이스케이프 기능을 제공하는 추상 클래스
    /// </summary>
    public class PacketHelper
    {
        // CRC-16 계산 (Modbus RTU 등)
        protected ushort ComputeCrc16(byte[] data, int offset, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = offset; i < offset + length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                    crc = (ushort)((crc & 1) != 0 ? (crc >> 1) ^ 0xA001 : crc >> 1);
            }
            return crc;
        }

        // LRC 계산 (Modbus ASCII 등)
        protected byte ComputeLrc(byte[] data, int offset, int length)
        {
            byte sum = 0;
            for (int i = offset; i < offset + length; i++)
                unchecked { sum += data[i]; }
            return (byte)((sum ^ 0xFF) + 1);
        }

        // CRC-32 계산 (Ethernet, ZIP 등)
        // (표 기반으로 0xEDB88320 다항식을 사용)
        private static readonly uint[] Crc32Table;

        // 정적 생성자에서 테이블 초기화
        static PacketHelper()
        {
            const uint polynomial = 0xEDB88320;
            Crc32Table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                    crc = (crc & 1) != 0
                        ? (crc >> 1) ^ polynomial
                        : crc >> 1;
                Crc32Table[i] = crc;
            }
        }

        /// <summary>
        /// data[offset]부터 length 바이트를 읽어 CRC-32를 계산하여 반환합니다.
        /// </summary>
        protected uint ComputeCrc32(byte[] data, int offset, int length)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = offset; i < offset + length; i++)
            {
                crc = (crc >> 8) ^ Crc32Table[(crc ^ data[i]) & 0xFF];
            }
            return crc ^ 0xFFFFFFFF;
        }

        // 필요하다면 이스케이프, 프레임 구분자 헬퍼 등 추가
    }
}
