using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Communication
{
    /// <summary>
    /// 통신 전송 수단(Serial, Socket 등)의 공통 기능을 정의합니다.
    /// </summary>
    public interface ICommunication : IDisposable
    {
        /// <summary>이 채널에 대한 설정 정보</summary>
        ICommunicationConfig Config { get; set; }

        /// <summary>데이터 수신 시 발생하는 이벤트</summary>
        event EventHandler<CommunicationEventArgs>? CommunicationEvent;

        /// <summary>현재 연결이 열려 있는지 여부</summary>
        bool IsOpen { get; }

        /// <summary>연결을 비동기로 엽니다.</summary>
        Task OpenAsync(CancellationToken cancellationToken = default);

        /// <summary>연결을 비동기로 닫습니다.</summary>
        Task CloseAsync(CancellationToken cancellationToken = default);

        /// <summary>데이터를 비동기로 전송합니다.</summary>
        Task SendAsync(byte[] data, CancellationToken cancellationToken = default);

        Task<byte[]?> SendReceiveAsync(byte[] data, Func<byte[], bool> responseMatcher, int timeoutMs = 1000);

        Task OnBgpacketAsync(CancellationToken cancellationToken = default);
        Task OffBgpacketAsync(CancellationToken cancellationToken = default);
        Task OnDoworkAsync(CancellationToken cancellationToken = default);
        Task OffDoworkAsync(CancellationToken cancellationToken = default);

        List<string> MethodList { get; set; }

        Task CallMethodAsync(string methodName, params object[] args);

        Task InitializeAsync();

    }

    public interface ICommunicationConfig
    {
        string CommunicationName { get; set; }
        /// <summary>통신 방식</summary>
        CommunicationTarget Target { get; set; }

        // Serial 전용 설정
        String PortName { get; set; }
        int BaudRate { get; set; }
        Parity Parity { get; set; }
        int DataBits { get; set; }
        StopBits StopBits { get; set; }

        // TCP 전용 설정
        string Host { get; set; }
        int Port { get; set; }

        bool BackgroundPacket { get; set; }

    }

    public interface IModbusConfig
    {
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        int RetryCount { get; set; }        
    }

    public interface IDataProvider
    {
        object? Data { get; }
    }


    public interface IModbusMasterWrapper
    {
        Task<bool[]> ReadCoilsAsync(ushort addr, ushort len, byte slaveId = 1);
        Task<bool[]> ReadInputsAsync(ushort addr, ushort len, byte slaveId = 1);
        Task<ushort[]> ReadHoldingRegistersAsync(ushort addr, ushort len, byte slaveId = 1);
        Task<ushort[]> ReadInputRegistersAsync(ushort addr, ushort len, byte slaveId = 1);

        Task<bool> WriteSingleCoilAsync(ushort addr, bool val, byte slaveId = 1);
        Task<bool> WriteMultipleCoilsAsync(ushort addr, bool[] vals, byte slaveId = 1);
        Task<bool> WriteSingleRegisterAsync(ushort addr, ushort val, byte slaveId = 1);
        Task<bool> WriteMultipleRegistersAsync(ushort addr, ushort[] vals, byte slaveId = 1);
    }
}
