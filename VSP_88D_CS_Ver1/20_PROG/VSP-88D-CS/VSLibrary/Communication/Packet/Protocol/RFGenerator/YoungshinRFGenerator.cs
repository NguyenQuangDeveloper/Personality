using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Communication.Serial;

namespace VSLibrary.Communication.Packet.Protocol.RFGenerator
{
    public partial class YoungshinRFGeneratorData : ObservableObject
    {
        [ObservableProperty] private bool _rfOn;
        [ObservableProperty] private bool _pulseOn;
        [ObservableProperty] private bool _maxLimitEnabled;
        [ObservableProperty] private bool _lowLimitEnabled;
        [ObservableProperty] private int _forwardPower;
        [ObservableProperty] private int _reflectPower;
        [ObservableProperty] private int _setPoint;
        [ObservableProperty] private int _biasVoltage;
        [ObservableProperty] private int _maxLimit;
        [ObservableProperty] private int _lowLimit;
        [ObservableProperty] private double _pulseTime;
        [ObservableProperty] private int _pulseDuty;
        [ObservableProperty] private (char mode, string code) _status;
    }

    public class YoungshinRFGenerator : SerialBase, IYoungshinRFGenerator, IDataProvider
    {
        private const int DEFAULT_TIMEOUT = 500;
        public YoungshinRFGeneratorData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public YoungshinRFGenerator(ICommunicationConfig config) : base(config)
        {
            _delimiter = new byte[] { 0x0D };
        }

        public override async Task InitializeAsync() { }

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
                        //await QueryStatusAsync();
                        //await QueryForwardPowerAsync();
                        //await QueryReflectPowerAsync();
                        //await QuerySetPointAsync();
                        //await QueryBiasVoltageAsync();
                        //await QueryMaxLimitAsync();
                        //await QueryLowLimitAsync();
                        //await QueryPulseTimeAsync();
                        await QueryPulseDutyAsync();
                    }
                }
                catch (Exception ex)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, $"상태 감시 오류: {ex.Message}");
                }

                var delay = interval - (DateTime.UtcNow - start);
                if (delay > TimeSpan.Zero)
                {
                    try { await Task.Delay(delay, token); } catch (TaskCanceledException) { }
                }
            }
        }

        protected override void OnPacket(byte[] packet, bool background = true)
        {
            var msg = Encoding.ASCII.GetString(packet).Trim();
            if (!Config.BackgroundPacket) return;

            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"RX: {msg}");

            if (msg.Length >= 3 && "SPKA".Contains(msg[0]))
                Data.Status = (msg[0], msg.Substring(1));
            else if (msg == "RON") Data.RfOn = true;
            else if (msg == "ROF") Data.RfOn = false;
            else if (msg == "PON") Data.PulseOn = true;
            else if (msg == "POF") Data.PulseOn = false;
            else if (msg == "MON") Data.MaxLimitEnabled = true;
            else if (msg == "MOF") Data.MaxLimitEnabled = false;
            else if (msg == "LON") Data.LowLimitEnabled = true;
            else if (msg == "LOF") Data.LowLimitEnabled = false;
            else
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"알 수 없는 응답: {msg}");
        }

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

        // ───── 제어 명령 ─────

        public Task TurnRfOnAsync() => SendCommandAsync("RON");
        public Task TurnRfOffAsync() => SendCommandAsync("ROF");
        public Task PulseOnAsync() => SendCommandAsync("PON");
        public Task PulseOffAsync() => SendCommandAsync("POF");
        public Task EnableMaxLimitAsync() => SendCommandAsync("MON");
        public Task DisableMaxLimitAsync() => SendCommandAsync("MOF");

        public Task SetPowerAsync(int w) => SendCommandAsync($"SP_{w:0000}");
        public Task SetPulseTimeAsync(double s) => SendCommandAsync($"PT_{s:0.0000}");
        public Task SetPulseDutyAsync(int pct) => SendCommandAsync($"PD_{pct:00}");
        public Task SetMaxLimitAsync(int w) => SendCommandAsync($"MX_{w:0000}");
        public Task SetLowLimitAsync(int w) => SendCommandAsync($"LO_{w:0000}");

        // ───── 조회 명령 ─────

        public async Task<(char, string)> QueryStatusAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("ST?"); return Data.Status; }

            var resp = await SendReceiveAsync("ST?", m => m.Length >= 3 && "SPKA".Contains(m[0]));
            if (resp != null) OnPacket(resp, false);
            return Data.Status;
        }

        public async Task<int> QueryForwardPowerAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("FW?"); return Data.ForwardPower; }

            var resp = await SendReceiveAsync("FW?", m => m.EndsWith("W"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('W'), out var v)) Data.ForwardPower = v;
            }
            return Data.ForwardPower;
        }

        public async Task<int> QueryReflectPowerAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("RW?"); return Data.ReflectPower; }

            var resp = await SendReceiveAsync("RW?", m => m.EndsWith("W"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('W'), out var v)) Data.ReflectPower = v;
            }
            return Data.ReflectPower;
        }

        public async Task<int> QuerySetPointAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("SP?"); return Data.SetPoint; }

            var resp = await SendReceiveAsync("SP?", m => m.EndsWith("W"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('W'), out var v)) Data.SetPoint = v;
            }
            return Data.SetPoint;
        }

        public async Task<int> QueryBiasVoltageAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("BV?"); return Data.BiasVoltage; }

            var resp = await SendReceiveAsync("BV?", m => m.EndsWith("V"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('V'), out var v)) Data.BiasVoltage = v;
            }
            return Data.BiasVoltage;
        }

        public async Task<int> QueryMaxLimitAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("MX?"); return Data.MaxLimit; }

            var resp = await SendReceiveAsync("MX?", m => m.EndsWith("W"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('W'), out var v)) Data.MaxLimit = v;
            }
            return Data.MaxLimit;
        }

        public async Task<int> QueryLowLimitAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("LO?"); return Data.LowLimit; }

            var resp = await SendReceiveAsync("LO?", m => m.EndsWith("W"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('W'), out var v)) Data.LowLimit = v;
            }
            return Data.LowLimit;
        }

        public async Task<double> QueryPulseTimeAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("PT?"); return Data.PulseTime; }

            var resp = await SendReceiveAsync("PT?", m => m.EndsWith("S"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (double.TryParse(s.TrimEnd('S'), out var v)) Data.PulseTime = v;
            }
            return Data.PulseTime;
        }

        public async Task<int> QueryPulseDutyAsync(bool useReceive = true)
        {
            if (!useReceive) { await SendCommandAsync("PD?"); return Data.PulseDuty; }

            var resp = await SendReceiveAsync("PD?", m => m.EndsWith("%"));
            if (resp != null)
            {
                var s = Encoding.ASCII.GetString(resp).Trim();
                if (int.TryParse(s.TrimEnd('%'), out var v)) Data.PulseDuty = v;
            }
            return Data.PulseDuty;
        }
    }
}
