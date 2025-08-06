using VSLibrary.Communication.Packet.Protocol.RFGenerator;

/// <summary>
/// YSR-03HDP 전용 인터페이스
/// </summary>
public interface IYoungshinRFGenerator
{
    YoungshinRFGeneratorData Data { get; }

    // Control 명령
    Task TurnRfOnAsync();        // RON
    Task TurnRfOffAsync();       // ROF
    Task PulseOnAsync();         // PON
    Task PulseOffAsync();        // POF
    Task EnableMaxLimitAsync();  // MON
    Task DisableMaxLimitAsync(); // MOF

    Task SetPowerAsync(int watts);       // SP_xxxx
    Task SetPulseTimeAsync(double sec);  // PT_xxxxx
    Task SetPulseDutyAsync(int pct);     // PD_xx
    Task SetMaxLimitAsync(int watts);    // MX_xxxx
    Task SetLowLimitAsync(int watts);    // LO_xxxx

    // 조회 명령 (useReceive 플래그 추가)
    Task<(char mode, string code)> QueryStatusAsync(bool useReceive = true);      // ST?
    Task<int> QueryForwardPowerAsync(bool useReceive = true); // FW?
    Task<int> QueryReflectPowerAsync(bool useReceive = true); // RW?
    Task<int> QuerySetPointAsync(bool useReceive = true);     // SP?
    Task<int> QueryBiasVoltageAsync(bool useReceive = true);  // BV?
    Task<int> QueryMaxLimitAsync(bool useReceive = true);     // MX?
    Task<int> QueryLowLimitAsync(bool useReceive = true);     // LO?
    Task<double> QueryPulseTimeAsync(bool useReceive = true);    // PT?
    Task<int> QueryPulseDutyAsync(bool useReceive = true);    // PD?
}
