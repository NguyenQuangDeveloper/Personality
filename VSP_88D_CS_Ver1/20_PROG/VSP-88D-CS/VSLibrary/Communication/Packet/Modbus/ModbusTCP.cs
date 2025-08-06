using CommunityToolkit.Mvvm.ComponentModel;
using Modbus;
using Modbus.Device;
using Modbus.IO;
using Modbus.Serial;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication.Socket;

namespace VSLibrary.Communication.Packet.Modbus
{
    /// <summary>
    /// NModbus4를 이용한 Modbus TCP 통신 래퍼
    /// 예외 발생 시 로그를 남기고, 읽기/쓰기 헬퍼를 통해 결과를 반환합니다.
    /// </summary>
    public partial class ModbusTCP : SocketBase, IModbusConfig, IModbusMasterWrapper
    {
        private IModbusMaster _master;

        [ObservableProperty] private int _readTimeout = 1000;
        [ObservableProperty] private int _writeTimeout = 1000;
        [ObservableProperty] private int _retryCount = 1;

        public ModbusTCP(ICommunicationConfig cfg) : base(cfg) { }

        public override async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            ShouldAutoInitialize = false;
            await base.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (IsOpen)
            {
                _master = ModbusIpMaster.CreateIp(_client);

                if (_master.Transport is ModbusTransport transport)
                {
                    transport.ReadTimeout = ReadTimeout;
                    transport.WriteTimeout = WriteTimeout;
                    transport.Retries = RetryCount;
                }

                await InitializeAsync().ConfigureAwait(false);
                OnDoworkAsync();
            }
        }

        public override async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (IsOpen)
            {
                _master?.Dispose();
                await base.CloseAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        #region 헬퍼 메서드

        private async Task<T> ExecuteAsync<T>(Func<Task<T>> func, string operation)
        {
            if (!IsOpen || _master == null)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.UnexpectedEx,
                             $"Op={operation}, Port closed or master null.");
                return default!;
            }

            T result;
            try
            {
                result = await func().ConfigureAwait(false);
            }
            catch (SlaveException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.SlaveEx,
                             $"Op={operation}, Msg={ex.Message}");
                return default!;
            }
            catch (TimeoutException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.Timeout,
                             $"Op={operation}, Msg={ex.Message}");
                return default!;
            }
            catch (IOException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.IOEx,
                             $"Op={operation}, Msg={ex.Message}");
                return default!;
            }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.UnexpectedEx,
                             $"Op={operation}, Type={ex.GetType().Name}, Msg={ex.Message}");
                return default!;
            }

            // 성공 로그
            string payload = result switch
            {
                bool[] bits => string.Join(",", bits.Select(b => b ? "1" : "0")),
                ushort[] regs => string.Join(",", regs),
                _ => result?.ToString() ?? "<null>"
            };
            EventMessage(Config.CommunicationName,
                         CommunicationEventType.Success,
                         $"Op={operation}, Result={payload}");

            return result;
        }

        private async Task<bool> ExecuteAsync(Func<Task> func, string operation)
        {
            if (!IsOpen || _master == null)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.UnexpectedEx,
                             $" Op={operation}, Port closed or master null.");
                return false;
            }

            try
            {
                await func().ConfigureAwait(false);
            }
            catch (SlaveException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.SlaveEx,
                             $"Op={operation}, Msg={ex.Message}");
                return false;
            }
            catch (TimeoutException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.Timeout,
                             $"Op={operation}, Msg={ex.Message}");
                return false;
            }
            catch (IOException ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.IOEx,
                             $"Op={operation}, Msg={ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName,
                             CommunicationEventType.UnexpectedEx,
                             $"Op={operation}, Type={ex.GetType().Name}, Msg={ex.Message}");
                return false;
            }

            EventMessage(Config.CommunicationName,
                         CommunicationEventType.Success,
                         $"Op={operation}");
            return true;
        }

        #endregion

        #region Read Methods

        public Task<bool[]> ReadCoilsAsync(
            ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadCoilsAsync(slaveId, startAddress, numberOfPoints),
                $"ReadCoils S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        public Task<bool[]> ReadInputsAsync(
            ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadInputsAsync(slaveId, startAddress, numberOfPoints),
                $"ReadInputs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        public Task<ushort[]> ReadHoldingRegistersAsync(
            ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadHoldingRegistersAsync(slaveId, startAddress, numberOfPoints),
                $"ReadHoldingRegs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        public Task<ushort[]> ReadInputRegistersAsync(
            ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadInputRegistersAsync(slaveId, startAddress, numberOfPoints),
                $"ReadInputRegs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        #endregion

        #region Write Methods

        public Task<bool> WriteSingleCoilAsync(
            ushort coilAddress, bool value, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteSingleCoilAsync(slaveId, coilAddress, value),
                $"WriteSingleCoil S={slaveId}, Addr={coilAddress}, Value={value}"
            );

        public Task<bool> WriteMultipleCoilsAsync(
            ushort startAddress, bool[] data, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteMultipleCoilsAsync(slaveId, startAddress, data),
                $"WriteMultipleCoils S={slaveId}, Addr={startAddress}, Len={data?.Length}"
            );

        public Task<bool> WriteSingleRegisterAsync(
            ushort registerAddress, ushort value, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteSingleRegisterAsync(slaveId, registerAddress, value),
                $"WriteSingleReg S={slaveId}, Addr={registerAddress}, Value={value}"
            );

        public Task<bool> WriteMultipleRegistersAsync(
            ushort startAddress, ushort[] data, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteMultipleRegistersAsync(slaveId, startAddress, data),
                $"WriteMultipleRegs S={slaveId}, Addr={startAddress}, Len={data?.Length}"
            );

        #endregion
    }
}
