using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Communication.Socket;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    // ───────────── Sample Data 클래스 ─────────────
    public partial class TestSocketData : ObservableObject
    {
        // 토글 상태
        [ObservableProperty] private bool _connected;
        [ObservableProperty] private bool _errorFlag;

        // 숫자 상태
        [ObservableProperty] private int _valueA;   // e.g. "VA:123"
        [ObservableProperty] private double _valueB; // e.g. "VB:45.67"
    }

    // ───────────── Sample 소켓 통신 클래스 ─────────────
    public class TestSocket : SocketBase, IDataProvider
    {
        private const int DEFAULT_TIMEOUT = 500;

        public TestSocketData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public TestSocket(ICommunicationConfig config)
            : base(config)
        {
            // 필요 시 패킷 구분자 설정 (_startSeq/_delimiter)
            _startSeq = null;
            _delimiter = Encoding.ASCII.GetBytes("\r\n");
        }

        public override async Task InitializeAsync()
        {
            //컨넥트시 실행되는 메소드
        }

        // 주기 폴링(상태 감시)
        protected override async Task DoWorkAsync()
        {
            var token = _doWorkCts.Token;
            var interval = TimeSpan.FromSeconds(1);

            while (!token.IsCancellationRequested)
            {
                var start = DateTime.UtcNow;

                try
                {
                    if (IsOpen)
                    {
                        //1초 이상 걸리면, `delay`는 0이거나 음수가 되어 즉시 다음 루프로 넘어갑니다 (오버런 발생).    
                        //await QueryConnectedAsync();
                        //await QueryValueAAsync();
                        await QueryValueBAsync();
                    }
                }
                catch (Exception ex)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, $"상태 감시 오류: {ex.Message}");
                }

                var elapsed = DateTime.UtcNow - start;
                var delay = interval - elapsed;

                if (delay > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(delay, token);
                    }
                    catch (TaskCanceledException) { }
                }
            }            
        }

        // 공통 패킷 처리
        protected override void OnPacket(byte[] packet, bool background = true)
        {
            var msg = Encoding.ASCII.GetString(packet).Trim();

            if (string.IsNullOrWhiteSpace(msg))
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, "빈 패킷 수신됨");
                return;
            }

            EventMessage(Config.CommunicationName,
                         background ? CommunicationEventType.Command : CommunicationEventType.Rx,
                         $"수신 패킷: {msg}");

            if (msg == "CONN_OK") Data.Connected = true;
            else if (msg == "CONN_FAIL") Data.Connected = false;
            else if (msg == "ERR1") Data.ErrorFlag = true;
            else if (msg == "ERR0") Data.ErrorFlag = false;
            else if (msg.StartsWith("VA:") && int.TryParse(msg[3..], out var a)) Data.ValueA = a;
            else if (msg.StartsWith("VB:") && double.TryParse(msg[3..], out var b)) Data.ValueB = b;
            else
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"알 수 없는 응답: {msg}");
        }

        // ───────────── 공통 헬퍼 ─────────────

        private async Task SendCommandAsync(string cmd)
        {
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"명령 전송: {cmd}");
            var b = Encoding.ASCII.GetBytes(cmd + "\r\n");
            await SendAsync(b);
        }

        private async Task<byte[]?> SendReceiveAsync(string cmd, Func<string, bool> match, int timeout = DEFAULT_TIMEOUT)
        {
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"요청: {cmd}");

            var b = Encoding.ASCII.GetBytes(cmd + "\r\n");
            var resp = await base.SendReceiveAsync(b, raw =>
            {
                var s = Encoding.ASCII.GetString(raw).Trim();
                return match(s);
            }, timeout);

            if (resp == null)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"응답 없음: {cmd}");
            }

            return resp;
        }

        // ───────────── 제어 명령 ─────────────

        public Task ConnectDeviceAsync() => SendCommandAsync("CONNECT");
        public Task DisconnectDeviceAsync() => SendCommandAsync("DISCONNECT");
        public Task ResetErrorAsync() => SendCommandAsync("ERR_RESET");

        // ───────────── 조회 명령 ─────────────

        public async Task<bool> QueryConnectedAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("STATUS?");
                return Data.Connected;
            }

            var resp = await SendReceiveAsync("STATUS?", s => s is "CONN_OK" or "CONN_FAIL");
            if (resp != null) OnPacket(resp, false);

            return Data.Connected;
        }

        public async Task<int> QueryValueAAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("VA?");
                return Data.ValueA;
            }

            var resp = await SendReceiveAsync("VA?", s => s.StartsWith("VA:"));
            if (resp != null) OnPacket(resp, false);

            return Data.ValueA;
        }

        public async Task<double> QueryValueBAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("VB?");
                return Data.ValueB;
            }

            var resp = await SendReceiveAsync("VB?", s => s.StartsWith("VB:"));
            if (resp != null) OnPacket(resp, false);

            return Data.ValueB;
        }
    }
}
