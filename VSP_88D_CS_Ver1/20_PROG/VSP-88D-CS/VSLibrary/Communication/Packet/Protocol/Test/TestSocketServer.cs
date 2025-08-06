using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication.Socket;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    // ───────────── Sample Server Data ─────────────
    public partial class TestSocketServerData : ObservableObject
    {
        [ObservableProperty] private bool _connected;
        [ObservableProperty] private bool _errorFlag;
        [ObservableProperty] private int _valueA;
        [ObservableProperty] private double _valueB;
    }

    // ───────────── Sample TCP Server Class ─────────────
    public class TestSocketServer : SocketServerBase, IDataProvider
    {
        private const int DEFAULT_TIMEOUT = 500;

        public TestSocketServerData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public TestSocketServer(ICommunicationConfig config)
            : base(config)
        {
            _startSeq = null;
            _delimiter = Encoding.ASCII.GetBytes("\r\n");
        }

        public override async Task InitializeAsync()
        {
            // 서버 초기화 시 필요한 작업이 있다면 여기서 수행
        }

        protected override async Task DoWorkAsync()
        {
            var token = _doWorkCts.Token;
            var interval = TimeSpan.FromSeconds(1);

            while (!token.IsCancellationRequested)
            {
                var start = DateTime.UtcNow;

                try
                {
                    if (IsOpen && IsClientConnected)
                    {
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

        // ───────────── 명령어 송신 및 응답 처리 ─────────────

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

        public Task ResetErrorAsync() => SendCommandAsync("ERR_RESET");

        // ───────────── 조회 명령 ─────────────

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
