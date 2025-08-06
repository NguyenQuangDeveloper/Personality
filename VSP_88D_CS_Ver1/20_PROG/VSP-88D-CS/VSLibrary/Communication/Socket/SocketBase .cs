using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication;

namespace VSLibrary.Communication.Socket
{
    /// <summary>
    /// TCP 소켓 통신 전용 추상 클래스
    /// (CommunicationBase 공통 로직 활용)
    /// </summary>
    public abstract class SocketBase : CommunicationBase
    {
        private readonly string _host;
        private readonly int _port;
        protected TcpClient _client = null!;
        private NetworkStream _stream = null!;
        private CancellationTokenSource _receiveLoopCts = null!;

        protected SocketBase(ICommunicationConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _host = config.Host;
            _port = config.Port;
        }

        /// <summary>
        /// 실제 쓰기: NetworkStream에 데이터 작성
        /// </summary>
        protected override async Task WriteCoreAsync(byte[] data, CancellationToken cancellationToken)
        {
            if (!IsOpen || _stream == null)
            {
                Console.WriteLine($"[SocketBase] SendAsync: 소켓이 열려있지 않음 ({_host}:{_port})");
                return;
            }
            try
            {
                await _stream.WriteAsync(data, 0, data.Length, CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                // 무시
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SocketBase] WriteCoreAsync 실패 ({_host}:{_port}): {ex.Message}");
            }
        }

        /// <summary>
        /// 연결 열기 및 수신/주기 작업 시작
        /// </summary>
        public override async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (IsOpen)
            {
                return;
            }

            try
            {
                _client = new TcpClient();
                using (cancellationToken.Register(() => _client.Close()))
                {
                    await _client.ConnectAsync(_host, _port);
                }
                _stream = _client.GetStream();
                IsOpen = true;                

                if (ShouldAutoInitialize)
                {
                    _receiveLoopCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    _ = Task.Run(() => ReceiveLoopAsync(_receiveLoopCts.Token), CancellationToken.None);

                    await InitializeAsync();
                    OnDoworkAsync();
                }
            }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.ConnectionError, $"OpenAsync 실패 ({_host}:{_port}): {ex.Message}");
                IsOpen = false;
            }
        }

        /// <summary>
        /// 연결 닫기 및 리소스 해제
        /// </summary>
        public override Task CloseAsync(CancellationToken cancellationToken = default)
        {
            // 주기 및 수신 루프 취소
            OffDoworkAsync();

            try
            {
                _receiveLoopCts?.Cancel();
                _receiveLoopCts?.Dispose();
            }
            catch { }

            try { _client?.Client?.Shutdown(SocketShutdown.Both); } catch { }
            try { _stream?.Dispose(); } catch { }
            try { _client?.Close(); } catch { }

            IsOpen = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 수신된 바이트를 CommunicationBase.ProcessReceivedBytes로 전달
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            var buffer = new byte[1024];
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead <= 0)
                    {
                        IsOpen = false;
                        await OffDoworkAsync(); // <-- 주기 작업 중단
                        EventMessage(Config.CommunicationName, CommunicationEventType.Disconnected, "서버 연결 끊김");
                        break;
                    }

                    var chunk = buffer.Take(bytesRead).ToArray();
                    ProcessReceivedBytes(chunk);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    IsOpen = false;
                    await OffDoworkAsync(); // <-- 에러로 종료되어도 중단
                    EventMessage(Config.CommunicationName, CommunicationEventType.RxError, $"ReceiveLoop Error ({_host}:{_port}): {ex.Message}");
                    break;
                }
            }
        }

        /// <summary>
        /// 필요 시 오버라이드
        /// </summary>
        protected virtual async Task DoWorkAsync() { }

        public override Task OnDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (_isWorking)
                return Task.CompletedTask;

            _doWorkCts = new CancellationTokenSource();
            _isWorking = true;
            _ = DoWorkAsync().ContinueWith(t =>
            {
                // 작업이 끝나거나 중단되면 상태 초기화
                _isWorking = false;
            }, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public override Task OffDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (!_isWorking || _doWorkCts == null || _doWorkCts.IsCancellationRequested)
                return Task.CompletedTask;

            try { _doWorkCts.Cancel(); } catch { }

            return Task.CompletedTask;
        }
    }
}
