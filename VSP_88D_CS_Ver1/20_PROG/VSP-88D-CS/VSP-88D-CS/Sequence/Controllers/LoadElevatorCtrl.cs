using DocumentFormat.OpenXml.Drawing.Charts;
using MySqlX.XDevAPI;
using SequenceEngine.Constants;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Controller;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Device;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Controllers;

public class LoadElevatorCtrl
{
    private ControllerManager _controller;
    private VS_GLOBAL_DATA _global_data;
    private IGlobalSystemOption _globalSystemOption;
    public IAxisData ElevatorZAxis { get; set; }
    public IAxisData ElevatorPusherXAxis { get; set; }

    public int MgzSlotIdx = -1;


    public LoadElevatorCtrl()
    {
        _controller = VSContainer.Instance.Resolve<ControllerManager>();
        _global_data = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
        _globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
        ElevatorZAxis = _controller.AxisData[(int)eMtr.Sm00_LoadElev_Z];
        ElevatorPusherXAxis = _controller.AxisData[(int)eMtr.Sm01_LoadPush_X];
    }

    public void StopServo()
    {
        ElevatorZAxis.StopMotion();
        ElevatorPusherXAxis.StopMotion();
    }

    public void ClearAlarmServo()
    {
        if (ElevatorZAxis.IsAlarm())
            ElevatorZAxis.ClearAlarm();

        if (ElevatorPusherXAxis.IsAlarm())
            ElevatorPusherXAxis.ClearAlarm();
    }

    public void EnableServo(bool status)
    {
        if (status && !ElevatorZAxis.IsServo())
        {
            ElevatorZAxis.SetServoOnOff(true);
        }
        else if (!status)
        {
            ElevatorZAxis.SetServoOnOff(false);
        }

        if (status && !ElevatorPusherXAxis.IsServo())
        {
            ElevatorPusherXAxis.SetServoOnOff(true);
        }
        else if (!status)
        {
            ElevatorPusherXAxis.SetServoOnOff(false);
        }
    }
    
    public bool StartZServo(ePositionLoadElevZ pos)
    {
        if (IsServoErr())
        {
            return false;
        }  
        if (IsStripJam())
        {
            return false;
        }   
        if (!IsXMotorPos(ePositionLoadElevPushX.RET))
        {
            //SendError(Err_LoadPush_NotRetPos);
            return false;
        }

        double targetPos, vel, acc;
        (targetPos, vel, acc) = _global_data.GetParamMotion((int)eMtr.Sm00_LoadElev_Z,(int)pos);

        if (pos == ePositionLoadElevZ.PITCH)
        {
            double slotPitch = _global_data.GetRecipe().GetPosition((int)eMtr.Sm00_LoadElev_Z, (int)pos);
            double slot1St = _global_data.GetRecipe().GetPosition((int)eMtr.Sm00_LoadElev_Z, (int)ePositionLoadElevZ.MGZ_BTM);
            int mgzSlotCnt = _global_data.GetRecipe().GetMgzSlotCnt();
            if (true/*_globalSystemOption.SystemItems.FromTopBtm*/)
            {
                targetPos = slot1St - (slotPitch * (mgzSlotCnt - MgzSlotIdx - 1));
            }
            else
            {
                targetPos = slot1St - (slotPitch * MgzSlotIdx);
            }
        }
        ElevatorZAxis.MoveToPoint(targetPos, vel, acc);
        return true;
    }
    public bool IsZMotorPos(ePositionLoadElevZ pos)
    {
        if (ElevatorZAxis.IsMoving())
        {
            return false;
        }

        double targetPos, vel, acc;
        (targetPos, vel, acc) = _global_data.GetParamMotion((int)eMtr.Sm00_LoadElev_Z, (int)pos);

        if (pos == ePositionLoadElevZ.PITCH)
        {
            double slotPitch = _global_data.GetRecipe().GetPosition((int)eMtr.Sm00_LoadElev_Z, (int)pos);
            double slot1St = _global_data.GetRecipe().GetPosition((int)eMtr.Sm00_LoadElev_Z, (int)ePositionLoadElevZ.MGZ_BTM);
            int mgzSlotCnt = _global_data.GetRecipe().GetMgzSlotCnt();
            if (true/*_globalSystemOption.SystemItems.FromTopBtm*/) 
            {
                targetPos = slot1St - (slotPitch * (mgzSlotCnt - MgzSlotIdx -1));
            }
            else
            {
                targetPos = slot1St - (slotPitch * MgzSlotIdx);
            }    
        }

        var currentPos = ElevatorZAxis.GetPosition();
        double tolerance = 0.001;
        bool isInRange = Math.Abs(currentPos - targetPos) <= tolerance;
        return isInRange;
    }

    public bool StartXServo(ePositionLoadElevPushX pos)
    {
        if(IsServoErr())
        {
            return false;
        }
        if (IsXMotorPos(pos))
        {
            return true;
        }
        double targetPos, vel, acc;
        (targetPos, vel, acc) = _global_data.GetParamMotion((int)eMtr.Sm01_LoadPush_X, (int)pos);

        ElevatorPusherXAxis.MoveToPoint(targetPos, vel, acc);
        return true;
    }

    public bool IsXMotorPos(ePositionLoadElevPushX pos)
    {
        if (ElevatorZAxis.IsMoving())
        {
            return false;
        }

        double targetPos, vel, acc;
        (targetPos, vel, acc) = _global_data.GetParamMotion((int)eMtr.Sm01_LoadPush_X, (int)pos);

        var currentPos = ElevatorZAxis.GetPosition();
        double tolerance = 0.001;
        bool isInRange = Math.Abs(currentPos - targetPos) <= tolerance;

        return isInRange;
    }
    public bool IsServoErr()
    {
        //Elevator Z
        if (!ElevatorZAxis.IsHomed())
            return true;

        if (ElevatorZAxis.IsAlarm())
            return true;

        //Load Pusher
        if (!ElevatorPusherXAxis.IsHomed())
            return true;

        if (ElevatorPusherXAxis.IsAlarm())
            return true;
        return false;
    }

    private bool IsStripJam()
    {
        if(DefinedDio.I_LoadElevStripChk_1.IsOn())
        {
            //SendError(Err_I_LoadElevStripChk_1);
            return true;
        }
        if(DefinedDio.I_LoadElevStripChk_2.IsOn()){
            //SendError(Err_I_LoadElevStripChk_2);
            return true;
        }
        if (DefinedDio.I_LoadElevStripChk_3.IsOn())
        {
            //SendError(Err_I_LoadElevStripChk_3);
            return true;
        }
        if (DefinedDio.I_LoadElevStripChk_4.IsOn())
        {
            //SendError(Err_I_LoadElevStripChk_4);
            return true;
        }
        if (DefinedDio.I_LoadElevStripChk_5.IsOn())
        { //2009-05-29
            //SendError(Err_I_LoadElevStripChk_5);
            return true;
        }
        return false;
    }

    private bool IsNoStripJam()
    {
       return (DefinedDio.I_LoadElevStripChk_1.IsOff() && DefinedDio.I_LoadElevStripChk_2.IsOff()
             && DefinedDio.I_LoadElevStripChk_3.IsOff() && DefinedDio.I_LoadElevStripChk_4.IsOff());
    }
}
