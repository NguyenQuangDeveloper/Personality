using System.Text;
using System.Threading;
using VSLibrary.Communication.Serial;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Xaml.Behaviors.Core;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.DirectoryServices.ActiveDirectory;

namespace VSLibrary.Communication.Packet.Protocol.RFGenerator
{
    public partial class PlasourceRfGeneratorData : ObservableObject, IPlasourceRfGeneratorData
    {
        [ObservableProperty] private ModeStatus mode;
        [ObservableProperty] private bool isRfOn;
        [ObservableProperty] private int forwardPower;
        [ObservableProperty] private int reflectPower;
        [ObservableProperty] private int settingPower;
        [ObservableProperty] private int faultStatus;
        [ObservableProperty] private byte address;
    }

    public class PlasourceRFGenerator : SerialBase, IPlasourceRfGenerator, IDataProvider
    {
        private const int DEFAULT_TIMEOUT = 300;

        private string _lastCmd;

        public PlasourceRfGeneratorData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public PlasourceRFGenerator(ICommunicationConfig config)
            : base(config)
        {
            _startSeq = null;
            //_startSeq = new byte[] { 0x02 };
            _delimiter = new byte[] { 0x0D }; // CR
        }

        public override async Task InitializeAsync()
        {
            var retryCount = 3;

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                var mode = await QueryModeAsync();

                if (mode == ModeStatus.DigitalRemote)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, "Remote 모드 감지 완료");
                    return;
                }

                var success = await EnableRemoteModeAsync();
                if (success)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, $"Remote 모드 전환 성공 (시도 {attempt})");
                    return;
                }

                // 간단한 딜레이 추가 (선택)
                await Task.Delay(200);
            }

            EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, "Remote 모드 전환 실패: 최대 재시도 초과");
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
                    if (IsOpen)
                    {
                        //1초 이상 걸리면, `delay`는 0이거나 음수가 되어 즉시 다음 루프로 넘어갑니다 (오버런 발생).    
                        //await QueryModeAsync();
                        //await QueryRfStatusAsync();
                        //await QueryForwardPowerAsync(false);
                        //await QueryReflectPowerAsync(false);
                        //await QuerySettingPowerAsync(false);
                        //await QueryStatusFaultAsync(true);
                        await QueryStatusFaultAsync(); // 작업 내용
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
            if (Config.BackgroundPacket || !background)
            {
                var msg = Encoding.ASCII.GetString(packet).Trim();

                // 에코 메시지인 경우 무시
                if (string.Equals(msg, _lastCmd, StringComparison.OrdinalIgnoreCase))
                    return;

                //EventMessage(Config.CommunicationName, CommunicationEventType.Rx, msg);
                //Console.WriteLine($"[PlasourceRFGenerator] RX: {msg}");

                if (msg == "RON") Data.IsRfOn = true;
                else if (msg == "ROF") Data.IsRfOn = false;
                else if (msg == "LOC") Data.Mode = ModeStatus.Local;
                else if (msg == "DSR") Data.Mode = ModeStatus.DigitalRemote;
                else if (msg == "ANA") Data.Mode = ModeStatus.Analog;
                else if (msg.StartsWith("FW") && int.TryParse(msg[2..], out var fw)) Data.ForwardPower = fw;
                else if (msg.StartsWith("RE") && int.TryParse(msg[2..], out var re)) Data.ReflectPower = re;
                else if (msg.StartsWith("SE") && int.TryParse(msg[2..], out var se)) Data.SettingPower = se;
                else if (msg.StartsWith("SF") && int.TryParse(msg[2..], out var sf)) Data.FaultStatus = sf;
                else
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"알 수 없는 응답: {msg}");
                    //Console.WriteLine($"[PlasourceRFGenerator] 알 수 없는 응답: {msg}");
                    ErrorMonitor();

                    return;
                }

                var prefix = background ? "백그라운드" : "동기";
                EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"{prefix} 패킷 처리: {msg}");
            }
        }

        // ─────────────── 헬퍼 메서드 ───────────────

        private async Task SendCommandAsync(string cmd)
        {
            _lastCmd = cmd;
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"명령 전송: {cmd}");
            var data = Encoding.ASCII.GetBytes(cmd + "\r"); //또는 // Encoding.ASCII.GetBytes(cmd + _delimiter);
            await SendAsync(data);
        }

        private async Task<bool> SendCommandAndWaitEchoAsync(string cmd)
        {
            _lastCmd = cmd;
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"명령 전송: {cmd}");

            var data = Encoding.ASCII.GetBytes(cmd + "\r");

            // 에코 자체를 matcher로 인정
            var resp = await base.SendReceiveAsync(data, raw =>
            {
                var msg = Encoding.ASCII.GetString(raw).Trim();
                return string.Equals(msg, cmd, StringComparison.OrdinalIgnoreCase);
            });

            if (resp == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// matcher는 해당 패킷이 유효한 데이터인지 확인 하는 펑션이 들어감.
        /// msg => msg.StartsWith("SE:") 패킷의 시작이 SE:로 시작할경우 유효한 데이터
        /// _ => true : 들어온 데이터는 모두 유효한 데이터 (리턴 받는 데이터를 가공해야함)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="matcher"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        private async Task<byte[]?> SendReceiveAsync(string cmd, Func<string, bool> matcher, int timeoutMs = DEFAULT_TIMEOUT)
        {
            //var data = Encoding.ASCII.GetBytes(cmd + "\r"); //또는 // Encoding.ASCII.GetBytes(cmd + _delimiter);
            //var respBytes = await base.SendReceiveAsync(data, raw =>
            //{
            //    var msg = Encoding.ASCII.GetString(raw).Trim();
            //    return matcher(msg);
            //});

            //return respBytes == null ? null : respBytes;

            _lastCmd = cmd;
            EventMessage(Config.CommunicationName, CommunicationEventType.Command, $"요청: {cmd}");
            var data = Encoding.ASCII.GetBytes(cmd + "\r");
            var resp = await base.SendReceiveAsync(data, raw =>
            {
                var msg = Encoding.ASCII.GetString(raw).Trim();

                // 에코인 경우 false → pending 보류
                if (string.Equals(msg, cmd, StringComparison.OrdinalIgnoreCase))
                    return false;

                return matcher(msg);
            }, timeoutMs);

            if (resp == null)
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"응답 없음: {cmd}");

            return resp;
        }

        // ─────────────── 제어 명령 ───────────────

        public Task<bool> EnableRemoteModeAsync() => SendCommandAndWaitEchoAsync("dre");
        public Task<bool> DisableRemoteModeAsync() => SendCommandAndWaitEchoAsync("drd");
        public Task<bool> TurnRfOnAsync() => SendCommandAndWaitEchoAsync("trg");
        public Task<bool> TurnRfOffAsync() => SendCommandAndWaitEchoAsync("off");
        public Task<bool> RegulationLoadAsync() => SendCommandAndWaitEchoAsync("rgl");
        public Task<bool> RegulationForwardAsync() => SendCommandAndWaitEchoAsync("rgf");
        public Task<bool> EnableExternalClockAsync() => SendCommandAndWaitEchoAsync("coe");
        public Task<bool> DisableExternalClockAsync() => SendCommandAndWaitEchoAsync("cod");

        public Task<bool> SetPowerAsync(int watts)
        {
            if (watts < 0 || watts > 1000)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.CommandError, $"잘못된 파워값: {watts}");
                //Console.WriteLine($"[SetPowerAsync] 잘못된 파워값: {watts}");
                return Task.FromResult(false);
            }

            return SendCommandAndWaitEchoAsync($"set{watts:0000}");
        }

        public Task<bool> SetAddressAsync(byte address)
        {
            Data.Address = address;
            return SendCommandAndWaitEchoAsync($"ads{address}");
        }

        // ─────────────── 조회 명령 ───────────────

        public async Task<ModeStatus> QueryModeAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?mo");
                return Data.Mode;
            }

            var resp = await SendReceiveAsync("?mo", msg => msg is "LOC" or "DSR" or "ANA");
            if (resp != null) OnPacket(resp, false);

            return Data.Mode;
        }

        public async Task<bool> QueryRfStatusAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?ro");
                return Data.IsRfOn;
            }

            var resp = await SendReceiveAsync("?ro", msg => msg is "RON" or "ROF");
            if (resp != null) OnPacket(resp, false);

            return Data.IsRfOn;
        }

        public async Task<int> QueryForwardPowerAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?fw");
                return Data.ForwardPower;
            }

            var resp = await SendReceiveAsync("?fw", msg => msg.StartsWith("FW"));
            if (resp != null) OnPacket(resp, false);

            return Data.ForwardPower;
        }

        public async Task<int> QueryReflectPowerAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?re");
                return Data.ReflectPower;
            }

            var resp = await SendReceiveAsync("?re", msg => msg.StartsWith("RE"));
            if (resp != null) OnPacket(resp, false);

            return Data.ReflectPower;
        }

        public async Task<int> QuerySettingPowerAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?se");
                return Data.SettingPower;
            }

            var resp = await SendReceiveAsync("?se", msg => msg.StartsWith("SE"));
            if (resp != null) OnPacket(resp, false);

            return Data.SettingPower;
        }

        public async Task<int> QueryStatusFaultAsync(bool useReceive = true)
        {
            if (!useReceive)
            {
                await SendCommandAsync("?sf");
                return Data.FaultStatus;
            }

            var resp = await SendReceiveAsync("?sf", msg => msg.StartsWith("SF"));
            if (resp != null) OnPacket(resp, false);

            return Data.FaultStatus;
        }
    }
}
