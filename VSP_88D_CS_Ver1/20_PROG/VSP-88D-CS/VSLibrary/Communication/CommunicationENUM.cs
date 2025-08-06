using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Communication
{
    /// <summary>
    /// 통신 대상 식별자 열거형
    /// </summary>
    public enum CommunicationTarget
    {
        TestSerial,
        TestSocket,
        TestSocketServer,
        TestModbusASCII,
        TestModbusRTU,
        TestModbusTCP,
        PlasourceRFGenerator,
        YoungshinRFGenerator,
    }

    public enum CommunicationEventType
    {
        // 연결 상태
        Connected,
        Disconnected,
        ConnectionError,

        // 데이터 송수신
        Tx,         // 송신 성공
        Rx,          // 수신 성공
        TxError,    // 송신 실패
        RxError,     // 수신 실패
        Inspection,  // 점검 필요

        // 타임아웃
        Timeout,          // 일반 타임아웃
        RxTimeout,   // 수신 지연
        TxTimeout,  // 송신 지연

        Command,
        CommandError,

        // 일반 오류
        Error,            // 정의되지 않은 오류
        ProtocolError,    // 프로토콜 위반 등 패킷 이상

        // 사용자 정의
        ManualDisconnect, // 사용자가 명시적으로 연결 끊음
        Retry,             // 재시도 발생

        //모드버스
        SlaveEx,  
        IOEx,
        UnexpectedEx,
        ModbusTimeout,
        Success,

    }
}
