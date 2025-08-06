using VSLibrary.Controller;

namespace VSP_88D_CS.CONTROLLER.DigitalIO;

public class AxisSetting : IAxisSettinglist
{
    public ControllerType ControllerType { get; set; }
    public short AxisNo { get; set; }
    public string AxisName { get; set; } = string.Empty;
    public string StrAxisData { get; set; } = string.Empty;
    public double LeadPitch { get; set; }
    public double PulsesPerRev { get; set; }
    public double GearRatio { get; set; }
    public byte PulseOutputMode { get; set; }
    public byte EncInputMode { get; set; }
    public double MinSpeed { get; set; }
    public double MaxSpeed { get; set; }
    public bool ServoEnabledReversal { get; set; }
    public bool LvSet_EndLimitP { get; set; }
    public bool LvSet_EndLimitN { get; set; }
    public bool LvSet_SlowLimitP { get; set; }
    public bool LvSet_SlowLimitN { get; set; }
    public bool LvSet_InPosition { get; set; }
    public bool LvSet_Alarm { get; set; }
}
