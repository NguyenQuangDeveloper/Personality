using VSLibrary.Common.MVVM.Core;
using VSLibrary.Controller;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Helpers;

namespace VSP_88D_CS.Sequence.Controllers;

public class LoadBufferCtrl
{
    private ControllerManager _controller;

    public LoadBufferCtrl()
    {
        _controller = VSContainer.Instance.Resolve<ControllerManager>();
    }

    public bool LoadStripCompleted()
    {
        return false;
    }

    public bool IsStopperDown(int timeOn)
    {
        return DefinedDio.I_InBufStopperDn.IsOn(timeOn);
    }

    public bool IsStopperUp(int timeOn)
    {
        return DefinedDio.I_InBufStopperUp.IsOn(timeOn);
    }
}
