using VSLibrary.Common.MVVM.Core;
using VSLibrary.Controller;
using VSP_88D_CS.Common;
using VSP_88D_CS.CONTROLLER;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Helpers;
using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Controllers;

public class LoadIndexPusherCtrl
{
    private ControllerManager _controller;

    private VS_GLOBAL_DATA _globalData;

    public IAxisData IndexPusherAxis { get; set; }

    public LoadIndexPusherCtrl()
    {
        _controller = VSContainer.Instance.Resolve<ControllerManager>();
        _globalData = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
        IndexPusherAxis = _controller.AxisData[(int)eMtr.Sm02_Index_X];
    }

    public void StopServo()
    {
        IndexPusherAxis.StopMotion();
    }

    public void ClearAlarmServo()
    {
        if(IndexPusherAxis.IsAlarm())
            IndexPusherAxis.ClearAlarm();
    }

    public void EnableServo(bool status)
    {
        if(status && !IndexPusherAxis.IsServo())
        {
            IndexPusherAxis.SetServoOnOff(true);
        }
        else if(!status)
        {
            IndexPusherAxis.SetServoOnOff(false);
        }
    }

    public bool StartXServo(ePositionIndexPusher pos)
    {
        if (IsServoErr())
            return false;

        if (IsXMotorPos(pos)) return true;

        if (DefinedDio.I_ChamberUp_Pm1.IsOff() &&
            (pos == ePositionIndexPusher.LOAD_END || pos == ePositionIndexPusher.UNLOAD_END
            || pos == ePositionIndexPusher.IN_EDGE || pos == ePositionIndexPusher.UNLOAD_START))
        {
            return false;
        }

        double targetPos, vel, acc;

        (targetPos, vel, acc) = _globalData.GetParamMotion((int)eMtr.Sm02_Index_X,(int)pos);

        IndexPusherAxis.MoveToPoint(targetPos, vel, acc);

        return true;
    }

    public bool IsXMotorPos(ePositionIndexPusher pos)
    {
        if (IndexPusherAxis.IsMoving()) return false;

        double targetPos, vel, acc;

        (targetPos, vel, acc) = _globalData.GetParamMotion((int)eMtr.Sm02_Index_X, (int)pos);

        var currentPos = IndexPusherAxis.GetPosition();

        double tolerance = 0.05; // TODO mm

        bool isInRange = Math.Abs(currentPos - targetPos) <= tolerance;

        return isInRange;
    }

    public bool IsServoErr()
    {
        if (!IndexPusherAxis.IsHomed())
            return true;

        if (IndexPusherAxis.IsAlarm())
            return true;

        return false;
    }

    public void SetPusherDown()
    {
        DefinedDio.O_Pusher2Up.Off();
        DefinedDio.O_Pusher2Dn.On();
    }

    public void SetPusherUp()
    {
        DefinedDio.O_Pusher2Dn.Off();
        DefinedDio.O_Pusher2Up.On();
    }

    public bool IsPusherUp(int time = 0)
    {
        return DefinedDio.I_Pusher2Up.IsOn(time);
    }

    public bool IsPusherDown(int time = 0)
    {
        return DefinedDio.I_Pusher2Dn.IsOn(time);
    }
}
