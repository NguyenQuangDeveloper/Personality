using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Text;
using VSLibrary.Communication.Packet.Modbus;
using VSLibrary.Communication.Serial;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    public partial class TestModbusAsciiData : ObservableObject
    {
        [ObservableProperty] private ushort[] holdingRegisters = [];
        [ObservableProperty] private ushort[] inputRegisters = [];
        [ObservableProperty] private bool[] coils = [];
        [ObservableProperty] private bool[] inputs = [];
    }

    public class TestModbusASCII : ModbusASCII, IDataProvider
    {
        public TestModbusAsciiData Data { get; } = new();
        object? IDataProvider.Data => Data;

        public TestModbusASCII(ICommunicationConfig config)
            : base(config)
        {
        }

        public override async Task InitializeAsync()
        {
            await QueryAllAsync();
        }

        protected override async Task DoWorkAsync()
        {
            //var token = _doWorkCts.Token;
            //var interval = TimeSpan.FromSeconds(1);

            //while (!token.IsCancellationRequested)
            //{
            //    var start = DateTime.UtcNow;

            //    try
            //    {
            //        if (IsOpen)
            //            await QueryAllAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        EventMessage(Config.CommunicationName, CommunicationEventType.Inspection, $"상태 감시 오류: {ex.Message}");
            //    }

            //    var elapsed = DateTime.UtcNow - start;
            //    var delay = interval - elapsed;

            //    if (delay > TimeSpan.Zero)
            //    {
            //        try
            //        {
            //            await Task.Delay(delay, token);
            //        }
            //        catch (TaskCanceledException) { }
            //    }
            //}
            var token = _doWorkCts.Token;
            var period = TimeSpan.FromSeconds(1);
            var sw = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                sw.Restart();

                try
                {
                    if (IsOpen)
                        await QueryAllAsync();
                }
                catch (Exception ex)
                {
                    EventMessage(Config.CommunicationName,
                                 CommunicationEventType.Inspection,
                                 $"상태 감시 오류: {ex.Message}");
                }

                // 남은 시간 계산
                var elapsed = sw.Elapsed;
                var remaining = period - elapsed;

                if (remaining > TimeSpan.Zero)
                {
                    // 1) 남은 시간에서 2ms 정도 제외하고 Task.Delay
                    var coarse = remaining - TimeSpan.FromMilliseconds(2);
                    if (coarse > TimeSpan.Zero)
                        await Task.Delay(coarse, token);

                    // 2) 마지막 2ms는 스핀 대기
                    while (sw.Elapsed < period)
                        Thread.SpinWait(10);
                }
            }

        }

        // ─────────────── 조회 명령 ───────────────

        public async Task<bool[]> QueryCoilsAsync(ushort addr = 0, ushort len = 8, byte slaveId = 1)
        {
            var result = await ReadCoilsAsync(addr, len, slaveId);
            Data.Coils = result;
            return result;
        }

        public async Task<bool[]> QueryInputsAsync(ushort addr = 0, ushort len = 8, byte slaveId = 1)
        {
            var result = await ReadInputsAsync(addr, len, slaveId);
            Data.Inputs = result;
            return result;
        }

        public async Task<ushort[]> QueryHoldingRegistersAsync(ushort addr = 0, ushort len = 4, byte slaveId = 1)
        {
            var result = await ReadHoldingRegistersAsync(addr, len, slaveId);
            Data.HoldingRegisters = result;
            return result;
        }

        public async Task<ushort[]> QueryInputRegistersAsync(ushort addr = 0, ushort len = 4, byte slaveId = 1)
        {
            var result = await ReadInputRegistersAsync(addr, len, slaveId);
            Data.InputRegisters = result;
            return result;
        }

        public async Task QueryAllAsync(byte slaveId = 1)
        {
            await QueryCoilsAsync(0, 8, slaveId);
            await QueryInputsAsync(0, 8, slaveId);
            await QueryHoldingRegistersAsync(0, 4, slaveId);
            await QueryInputRegistersAsync(0, 4, slaveId);
        }
    }
}
