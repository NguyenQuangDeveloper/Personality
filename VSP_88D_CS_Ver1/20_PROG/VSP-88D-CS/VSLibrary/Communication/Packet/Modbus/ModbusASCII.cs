using CommunityToolkit.Mvvm.ComponentModel;
using Modbus;
using Modbus.Device;
using Modbus.IO;
using Modbus.Serial;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication.Serial;

namespace VSLibrary.Communication.Packet.Modbus
{
    /// <summary>
    /// NModbus4를 이용한 Modbus ASCII 통신 래퍼
    /// 예외 발생 시 로그를 남기고 호출자에게 재전파합니다.
    /// </summary>
    public partial class ModbusASCII : SerialBase, IModbusConfig, IModbusMasterWrapper
    {
        private IModbusSerialMaster _master;

        [ObservableProperty]
        private int _readTimeout = 1000;

        [ObservableProperty]
        private int _writeTimeout = 1000;

        [ObservableProperty]
        private int _retryCount = 1;

        public ModbusASCII(ICommunicationConfig cfg)
            : base(cfg)
        {
            _serialPort.DataReceived -= DataReceivedHandler;

            ReadTimeout = 1000;
            WriteTimeout = 1000;
            RetryCount = 1;
        }

        public override async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            ShouldAutoInitialize = false;
            await base.OpenAsync(cancellationToken);

            if (IsOpen == true)
            {
                //Console.WriteLine(
                //$"[PortConfig] Baud={_serialPort.BaudRate}, " +
                //$"DataBits={_serialPort.DataBits}, Parity={_serialPort.Parity}, StopBits={_serialPort.StopBits}");
                EventMessage(Config.CommunicationName, CommunicationEventType.Connected, 
                    $"[PortConfig] Baud={_serialPort.BaudRate}, " +
                    $"DataBits={_serialPort.DataBits}, Parity={_serialPort.Parity}, StopBits={_serialPort.StopBits}");

                var adapter = new SerialPortAdapter(_serialPort);
                _master = ModbusSerialMaster.CreateAscii(adapter);

                if (_master.Transport is ModbusSerialTransport transport)
                {
                    transport.ReadTimeout = ReadTimeout;
                    transport.WriteTimeout = WriteTimeout;
                    transport.Retries = RetryCount;
                }

                await InitializeAsync();
                OnDoworkAsync();
            }
        }

        public override async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (IsOpen == true)
            {
                OffDoworkAsync();
                _master?.Dispose();
                //await base.CloseAsync(cancellationToken);                
                IsOpen = false;
                EventMessage(Config.CommunicationName, CommunicationEventType.Disconnected, $"Serial close {Config.PortName}");
            }
        }

        #region 헬퍼 메서드

        /// <summary>
        /// Modbus 호출부를 감싸는 공통 헬퍼
        /// </summary>
        private async Task<T> ExecuteAsync<T>(Func<Task<T>> func, string operation)
        {
            if (!IsOpen || _master == null)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.UnexpectedEx, $"Op={operation}, Port closed or master null.");
                return default;
            }

            T result;
            try
            {
                // 실제 Modbus 호출
                result = await func();
            }
            catch (SlaveException ex)
            {
                // (기존 예외 처리)
                byte[] err = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.SlaveEx, Encoding.ASCII.GetString(err));
                //OnDataReceived(err);
                return default;
            }
            catch (TimeoutException ex)
            {
                byte[] err = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.Timeout, Encoding.ASCII.GetString(err));
                //OnDataReceived(err);
                return default;
            }
            catch (IOException ex)
            {
                byte[] err = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.IOEx, Encoding.ASCII.GetString(err));
                //OnDataReceived(err);
                return default;
            }
            catch (Exception ex)
            {
                byte[] err = Encoding.ASCII.GetBytes(
                    $"Op={operation}, " +
                    $"Type={ex.GetType().Name}, Msg={ex.Message}"
                );
                EventMessage(Config.CommunicationName, CommunicationEventType.UnexpectedEx, Encoding.ASCII.GetString(err));
                //OnDataReceived(err);
                return default;
            }

            // ───────────────────────────────────────────────
            // 정상 응답도 로그로 올려준다!
            // bool[] / ushort[] 결과 모두 ToString() 으로 직렬화하거나,
            // 원하는 형식으로 포맷해서 찍어주시면 됩니다.
            // ───────────────────────────────────────────────
            string payload;
            switch (result)
            {
                case bool[] bits:
                    payload = string.Join(",", bits.Select(b => b ? "1" : "0"));
                    break;
                case ushort[] regs:
                    payload = string.Join(",", regs);
                    break;
                default:
                    payload = result?.ToString() ?? "<null>";
                    break;
            }

            byte[] ok = Encoding.ASCII.GetBytes($"Op={operation}, Result={payload}");
            EventMessage(Config.CommunicationName, CommunicationEventType.Success, Encoding.ASCII.GetString(ok));
            //OnDataReceived(ok);

            return result;
        }

        /// <summary>
        /// Modbus 쓰기 호출용 헬퍼
        /// </summary>
        private async Task<bool> ExecuteAsync(Func<Task> func, string operation)
        {
            if (!IsOpen || _master == null)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.UnexpectedEx, $"Op={operation}, Port closed or master null.");
                return false;
            }

            try
            {
                await func();                
            }
            catch (SlaveException ex)
            {
                byte[] data = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.SlaveEx, Encoding.ASCII.GetString(data));
                //OnDataReceived(data);
                return false;
            }
            catch (TimeoutException ex)
            {
                byte[] data = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.Success, Encoding.ASCII.GetString(data));
                //OnDataReceived(data);
                return false;
            }
            catch (IOException ex)
            {
                byte[] data = Encoding.ASCII.GetBytes($"Op={operation}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.IOEx, Encoding.ASCII.GetString(data));
                //OnDataReceived(data);
                return false;
            }
            catch (Exception ex)
            {
                byte[] data = Encoding.ASCII.GetBytes($"Op={operation}, Type={ex.GetType().Name}, Msg={ex.Message}");
                EventMessage(Config.CommunicationName, CommunicationEventType.UnexpectedEx, Encoding.ASCII.GetString(data));
                //OnDataReceived(data);
                return false;
            }

            // 성공 로그
            byte[] ok = Encoding.ASCII.GetBytes($"Op={operation}");
            EventMessage(Config.CommunicationName, CommunicationEventType.Success, Encoding.ASCII.GetString(ok));
            //OnDataReceived(ok);
            return true;
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// 0x01 Read Coils 호출
        /// 디지털 출력 상태 읽기 (1bit)
        /// </summary>
        public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadCoilsAsync(slaveId, startAddress, numberOfPoints),
                operation: $"ReadCoils S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        /// <summary>
        /// 0x02 Read Discrete Inputs 호출
        /// 디지털 입력 읽기 (1bit)
        /// </summary>
        public Task<bool[]> ReadInputsAsync(ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadInputsAsync(slaveId, startAddress, numberOfPoints),
                operation: $"ReadInputs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        /// <summary>
        /// 0x03 Read Holding Registers 호출
        /// 아날로그 출력 값 읽기 (16bit)
        /// </summary>
        public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadHoldingRegistersAsync(slaveId, startAddress, numberOfPoints),
                operation: $"ReadHoldingRegs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        /// <summary>
        /// 0x04 Read Input Registers 호출
        /// 아날로그 입력 값 읽기 (16bit)
        /// </summary>
        public Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort numberOfPoints, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.ReadInputRegistersAsync(slaveId, startAddress, numberOfPoints),
                operation: $"ReadInputRegs S={slaveId}, Addr={startAddress}, Len={numberOfPoints}"
            );

        #endregion

        #region Write Methods

        /// <summary>
        /// 0x05 Write Single Coil 호출
        /// 디지털 출력 1개 쓰기
        /// </summary>
        public Task<bool> WriteSingleCoilAsync(ushort coilAddress, bool value, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteSingleCoilAsync(slaveId, coilAddress, value),
                operation: $"WriteSingleCoil S={slaveId}, Addr={coilAddress}, Value={value}"
            );

        /// <summary>
        /// 0x0F Write Multiple Coils 호출
        /// 레지스터 1개 쓰기
        /// </summary>
        public Task<bool> WriteMultipleCoilsAsync(ushort startAddress, bool[] data, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteMultipleCoilsAsync(slaveId, startAddress, data),
                operation: $"WriteMultipleCoils S={slaveId}, Addr={startAddress}, Len={data?.Length}"
            );

        /// <summary>
        /// 0x06 Write Single Register 호출
        /// 디지털 출력 여러 개 쓰기
        /// </summary>
        public Task<bool> WriteSingleRegisterAsync(ushort registerAddress, ushort value, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteSingleRegisterAsync(slaveId, registerAddress, value),
                operation: $"WriteSingleReg S={slaveId}, Addr={registerAddress}, Value={value}"
            );

        /// <summary>
        /// 0x10 Write Multiple Registers 호출
        /// 레지스터 여러 개 쓰기
        /// </summary>
        public Task<bool> WriteMultipleRegistersAsync(ushort startAddress, ushort[] data, byte slaveId = 1) =>
            ExecuteAsync(
                () => _master.WriteMultipleRegistersAsync(slaveId, startAddress, data),
                operation: $"WriteMultipleRegs S={slaveId}, Addr={startAddress}, Len={data?.Length}"
            );

        #endregion
    }
}
