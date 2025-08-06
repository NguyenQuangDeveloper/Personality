using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Helpers;
using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Controllers;

public class PlasmaCtrl
{
    public bool IsPlasmaReady()
    {
        return false;
    }

    public bool IsPlasmaCompleted()
    {
        return StripManager.Instance.GetStripState(eStripState.PLASMAED) && IsChamberUp();
    }

    public bool IsChamberUp(int timeOn = 0)
    {
        return DefinedDio.I_ChamberUp_Pm1.IsOn(timeOn);
    }
}
