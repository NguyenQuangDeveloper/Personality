using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace VSLibrary.Communication.Packet.Protocol.RFGenerator
{
    public interface IPlasourceRfGenerator
    {
        PlasourceRfGeneratorData Data { get; }

        // ──────────────── 제어 명령 ────────────────

        Task<bool> EnableRemoteModeAsync();
        Task<bool> DisableRemoteModeAsync();
        Task<bool> TurnRfOnAsync();
        Task<bool> TurnRfOffAsync();
        Task<bool> RegulationLoadAsync();
        Task<bool> RegulationForwardAsync();
        Task<bool> EnableExternalClockAsync();
        Task<bool> DisableExternalClockAsync();
        Task<bool> SetPowerAsync(int watts);
        Task<bool> SetAddressAsync(byte address);

        // ──────────────── 상태 조회 명령 (OnPacket fallback 옵션 추가) ────────────────

        Task<ModeStatus> QueryModeAsync(bool useFallback = true);
        Task<bool> QueryRfStatusAsync(bool useFallback = true);
        Task<int> QueryForwardPowerAsync(bool useFallback = true);
        Task<int> QueryReflectPowerAsync(bool useFallback = true);
        Task<int> QuerySettingPowerAsync(bool useFallback = true);
        Task<int> QueryStatusFaultAsync(bool useFallback = true);
    }

    public enum ModeStatus
    {
        Unknown = 0,
        Local,
        DigitalRemote,
        Analog
    }

    public interface IPlasourceRfGeneratorData : INotifyPropertyChanged
    {
        ModeStatus Mode { get; set; }
        bool IsRfOn { get; set; }
        int ForwardPower { get; set; }
        int ReflectPower { get; set; }
        int SettingPower { get; set; }
        int FaultStatus { get; set; }
        byte Address { get; set; }
    }
}
