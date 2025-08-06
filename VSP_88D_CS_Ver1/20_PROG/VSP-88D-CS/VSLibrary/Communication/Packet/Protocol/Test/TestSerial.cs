using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Communication.Serial;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    // ───────────── 상태 데이터 클래스 ─────────────
    public partial class TestSerialData : ObservableObject
    {
        [ObservableProperty] private bool powerOn;
        [ObservableProperty] private bool alarmRaised;
        [ObservableProperty] private int voltage;         // 예: VLT123V
        [ObservableProperty] private double temperature;  // 예: TMP25.5C        
    }

    // ───────────── 통신 클래스 ─────────────
    public class TestSerial : SerialBase, IDataProvider
    {
        private const int DEFAULT_TIMEOUT = 500;

        public TestSerialData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public TestSerial(ICommunicationConfig config) : base(config)
        {
            _startSeq = null;
            _delimiter = Encoding.ASCII.GetBytes("\r\n");
            //_ = DoWorkAsync();
        }

        protected override async Task DoWorkAsync()
        {
            var token = _doWorkCts.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (IsOpen)
                    {
                        await QueryPowerAsync();
                        await QueryVoltageAsync();
                        await QueryTemperatureAsync();
                    }
                }
                catch (Exception ex)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, $"상태 감시 오류: {ex.Message}");
                }

                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException) { }
            }
        }

        protected override void OnPacket(byte[] packet, bool background = true)
        {
            var msg = Encoding.ASCII.GetString(packet).Trim();

            if (msg == "PWRON") Data.PowerOn = true;
            else if (msg == "PWROFF") Data.PowerOn = false;
            else if (msg == "ALARM1") Data.AlarmRaised = true;
            else if (msg == "ALARM0") Data.AlarmRaised = false;
            else if (msg.StartsWith("VLT") && int.TryParse(msg[3..^1], out var v)) Data.Voltage = v;
            else if (msg.StartsWith("TMP") && double.TryParse(msg[3..^1], out var t)) Data.Temperature = t;
            else
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"알 수 없는 응답: {msg}");
                return;
            }

            var prefix = background ? "백그라운드" : "동기";
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"{prefix} 패킷 처리: {msg}");
        }

        // ───────────── 헬퍼 메서드 ─────────────

        private async Task SendCommandAsync(string cmd)
        {
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"명령 전송: {cmd}");
            var b = Encoding.ASCII.GetBytes(cmd + "\r");
            await SendAsync(b);
        }        

        private async Task<byte[]?> SendReceiveAsync(string cmd, Func<string, bool> match, int timeout = DEFAULT_TIMEOUT)
        {
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"요청: {cmd}");
            var b = Encoding.ASCII.GetBytes(cmd + "\r");
            var resp = await base.SendReceiveAsync(b, raw =>
            {
                var s = Encoding.ASCII.GetString(raw).Trim();
                return match(s);
            }, timeout);

            if (resp == null)
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"응답 없음: {cmd}");

            return resp;
        }

        // ───────────── 제어 명령 ─────────────

        public Task TurnOnAsync() => SendCommandAsync("PWRON");
        public Task TurnOffAsync() => SendCommandAsync("PWROFF");
        public Task ResetAlarmAsync() => SendCommandAsync("ALARM0");

        // ───────────── 조회 명령 ─────────────

        public async Task<bool> QueryPowerAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("PWR?");
                return Data.PowerOn;
            }

            var resp = await SendReceiveAsync("PWR?", s => s is "PWRON" or "PWROFF");
            if (resp != null) OnPacket(resp, false);

            return Data.PowerOn;
        }

        public async Task<int> QueryVoltageAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("VLT?");
                return Data.Voltage;
            }

            var resp = await SendReceiveAsync("VLT?", s => s.StartsWith("VLT") && s.EndsWith("V"));
            if (resp != null) OnPacket(resp, false);

            return Data.Voltage;
        }

        public async Task<double> QueryTemperatureAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("TMP?");
                return Data.Temperature;
            }

            var resp = await SendReceiveAsync("TMP?", s => s.StartsWith("TMP") && s.EndsWith("C"));
            if (resp != null) OnPacket(resp, false);

            return Data.Temperature;
        }
    }
}
