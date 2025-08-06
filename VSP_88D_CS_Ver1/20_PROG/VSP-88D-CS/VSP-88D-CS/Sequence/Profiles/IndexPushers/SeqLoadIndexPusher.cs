using LoggerLib.Interfaces;
using SequenceEngine.Bases;
using SequenceEngine.Constants;
using SequenceEngine.Restore;
using System.Printing;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Controller;
using VSP_88D_CS.Common;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Helpers;
using VSP_88D_CS.Sequence.Constants;
using VSP_88D_CS.Sequence.Controllers;

namespace VSP_88D_CS.Sequence.Profiles.IndexPushers;

public class SeqLoadIndexPusher : BaseSequence
{
    public override int ModuleId { get; set; } = (int)eSequenceModule.SeqLoadIndexPusher;
    public override string LogHead { get; set; } = "LOAD_INDEX_PUSHER";
   
    private VS_GLOBAL_DATA _globalData;
    
    private ILoggingService _logger;

    public SeqLoadIndexPusher()
    {
        _globalData = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
        _logger = VSContainer.Instance.Resolve<ILoggingService>();
        InitStep();
    }

    protected override void Restore(SequenceRestore state)
    {
        base.Restore(state);
    }

    public override void Stop()
    {
        base.Stop();

        BaseCtrl.Instance.IndexPusherCtrl.StopServo();
    }
    
    public override void ClearAlarm()
    {
        base.ClearAlarm();
        BaseCtrl.Instance.IndexPusherCtrl.ClearAlarmServo();
    }

    public override void AlwaysRun()
    {
        base.AlwaysRun();

        CheckServoEnable();
        CheckServoLimit();
    }

    public override bool Initialize()
    {
        if (IsInitialized) return true;

        if(BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.IsHomed() && 
            BaseCtrl.Instance.IndexPusherCtrl.IsPusherUp())
        {
            IsInitialized = true;
            return IsInitialized;
        }

        if(!BaseCtrl.Instance.IndexPusherCtrl.IsPusherUp())
            BaseCtrl.Instance.IndexPusherCtrl.SetPusherUp();

        if (BaseCtrl.Instance.PlasmaCtrl.IsChamberUp() && 
            !BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.IsMoving()) 
        {
            var velocities = new double[] { 50.0, 2.0 };
            var accelerations = new double[] { 100.0, 100.0 };
            var offsets = new double[] { 0.0, 0,0 };
            var directions = new bool[] { false, false };
            var detectSignals = new byte[] { 0x4, 0x4 };

            Motion_HomeConfig homeConfig = new Motion_HomeConfig(
                velocities, accelerations, offsets, directions, detectSignals);


            BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.HomeMove(homeConfig);
        }
            
        return IsInitialized = true;
    }

    protected override void SetAlarm(int nErrorCode)
    {
        if (!_globalData.GetErrFlag())
        {
            _globalData.SetErrFlag(true);
            _logger.LogError(string.Format("{0}: Error No.{0} Step {0}", LogHead, nErrorCode, currentStep));
        }
        base.SetAlarm(nErrorCode);
    }

    public override eSequenceResult RunSequence()
    {
        if (!GetWork()) return eSequenceResult.NOT_READY;

        unitStep = GetUnitStep();

        switch ((eStep)currentStep)
        {
            case eStep.START:

                NextStep();
                break;

            case eStep.STEP_1_CHECK_READY:
                UnitStepCheck();
                break;

            case eStep.STEP_2_DIVERGENCE:
                if (BaseCtrl.Instance.PlasmaCtrl.IsPlasmaReady() &&
                    BaseCtrl.Instance.PlasmaCtrl.IsChamberUp() &&
                    StripIsLoadLocation())
                {
                    NextStep(eStep.STEP_10_LOAD_STRIP_TO_CHAMBER);
                }
                else if(BaseCtrl.Instance.PlasmaCtrl.IsPlasmaCompleted())
                {
                    NextStep(eStep.STEP_50_LOAD_STRIP_TO_UNLOAD);
                }    

                break;

            case eStep.STEP_10_LOAD_STRIP_TO_CHAMBER:
                if (BaseCtrl.Instance.PlasmaCtrl.IsPlasmaReady() && 
                    BaseCtrl.Instance.PlasmaCtrl.IsChamberUp())
                    NextStep();
                break;
            case eStep.Step_11_Servo_X_Load_Start:
            case eStep.Step_12_Cylinder_Down:
            case eStep.Step_13_Servo_X_Load_End:
            case eStep.Step_14_Servo_X_In_Edge:
            case eStep.Step_15_Cylinder_Up:
                UnitStepCheck();
                if (currentStep == (int)eStep.Step_15_Cylinder_Up)
                    return eSequenceResult.SUCCESS;

                break;

            case eStep.STEP_50_LOAD_STRIP_TO_UNLOAD:
                if(BaseCtrl.Instance.PlasmaCtrl.IsPlasmaCompleted() && 
                    BaseCtrl.Instance.PlasmaCtrl.IsChamberUp())
                    NextStep();
                break;
            case eStep.Step_51_Servo_X_Unload_Start:
            case eStep.Step_52_Cylinder_Down:
            case eStep.Step_53_Servo_X_Unload_End:
            case eStep.Step_54_Servo_Cylinder_Up:
            case eStep.Step_55_Servo_Servo_X_Ready:
                UnitStepCheck();
                if (currentStep == (int)eStep.Step_55_Servo_Servo_X_Ready)
                    return eSequenceResult.SUCCESS;

                break;

            case eStep.END:
                _logger.LogDebug("Seq Index Pusher Completed");
                NextStep(eStep.START);
                break;
        }

        return eSequenceResult.BUSY;
    }

    private void NextStep(eStep step = eStep.IDLE)
    {
        if (!GetWork()) return;
        base.NextStep((int)step);
        string log = string.Format("{0}", Enum.GetName(typeof(eStep), (eStep)currentStep));
        _logger.LogDebug(log);
    }

    private void InitStep()
    {
        actionUnitStep.Add((int)eStep.STEP_1_CHECK_READY, new UnitStep
        {
            CheckCondition = () =>
            {
                if (!BaseCtrl.Instance.PlasmaCtrl.IsChamberUp())
                    return false;

                return true;
            },
            ExecuteAction = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.READY);
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.READY);
            },
            NextStep = () =>
            {
                return (int)eStep.STEP_2_DIVERGENCE;
            } 
        });
        actionUnitStep.Add((int)eStep.Step_11_Servo_X_Load_Start, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if (!BaseCtrl.Instance.LoadBufferCtrl.IsStopperDown(100))
                    return false;

                return true;
            },
            ExecuteAction = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.LOAD_START);
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.LOAD_START);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_12_Cylinder_Down;
            }
        });
        actionUnitStep.Add((int)eStep.Step_12_Cylinder_Down, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if (!BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.LOAD_START))
                {
                    //SetAlarm("TODO");
                    return false;
                }    
                    
                if (!BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.UNLOAD_START))
                {
                    //SetAlarm("TODO");
                    return false;
                }    

                return true;
            },
            ExecuteAction = () =>
            {
                BaseCtrl.Instance.IndexPusherCtrl.SetPusherDown();
                return true;
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsPusherDown(100);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_I_Pusher2Dn);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_13_Servo_X_Load_End;
            }
        });
        actionUnitStep.Add((int)eStep.Step_13_Servo_X_Load_End, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if (!BaseCtrl.Instance.LoadBufferCtrl.IsStopperDown(100))
                {
                    SetAlarm((int)eErrCode.Ecode_I_InBufStopperDn);
                    return false;
                }    

                return true;
            },
            ExecuteAction = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.LOAD_END);
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.LOAD_END);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_14_Servo_X_In_Edge;
            }
        });
        actionUnitStep.Add((int)eStep.Step_14_Servo_X_In_Edge, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if (!BaseCtrl.Instance.LoadBufferCtrl.IsStopperDown(100))
                {
                    SetAlarm((int)eErrCode.Ecode_I_InBufStopperDn);
                    return false;
                }

                return true;
            },
            ExecuteAction = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.IN_EDGE);
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.IN_EDGE);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_15_Cylinder_Up;
            }
        });
        actionUnitStep.Add((int)eStep.Step_15_Cylinder_Up, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                return true;
            },
            ExecuteAction = () =>
            {
                SetStripLocation(eStripLocation.IN_CHAMBER);
                BaseCtrl.Instance.IndexPusherCtrl.SetPusherUp();
                return true;
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsPusherUp(100);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_I_Pusher2Up);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_16_Loading_Complete;
            }
        });
        actionUnitStep.Add((int)eStep.Step_16_Loading_Complete, new UnitStep()
        {
            CheckCondition = () =>
            {
                if (DefinedDio.I_Pusher2StripChk_1.IsOn())
                {
                    SetAlarm((int)eErrCode.Ecode_I_Pusher2StripChk_1);
                    return false;
                }
                else if (DefinedDio.I_Pusher2StripChk_2.IsOn())
                {
                    SetAlarm((int)eErrCode.Ecode_I_Pusher2StripChk_2);
                    return false;
                }
                else if (DefinedDio.I_Pusher2StripChk_3.IsOn())
                {
                    SetAlarm((int)eErrCode.Ecode_I_Pusher2StripChk_3);
                    return false;
                }
                else if (DefinedDio.I_Pusher2StripChk_4.IsOn())
                {
                    SetAlarm((int)eErrCode.Ecode_I_Pusher2StripChk_4);
                    return false;
                }
                else if (DefinedDio.I_Pusher2StripChk_5.IsOn())
                {
                    SetAlarm((int)eErrCode.Ecode_I_Pusher2StripChk_5);
                    return false;
                }
                return true;
            },
            ExecuteAction = () =>
            {
                return true;
            },
            IsStateOK = () =>
            {
                return true;
            },
            NextStep = () =>
            {
                return (int)eStep.STEP_2_DIVERGENCE;
            }
        });
        actionUnitStep.Add((int)eStep.Step_51_Servo_X_Unload_Start, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if(!BaseCtrl.Instance.PlasmaCtrl.IsChamberUp())
                {
                    SetAlarm((int)eErrCode.Ecode_I_ChamberUp_Pm1);
                    return false;
                }    
                return true;
            },
            ExecuteAction = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.UNLOAD_START);
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.UNLOAD_START);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_52_Cylinder_Down;
            }
        });
        actionUnitStep.Add((int)eStep.Step_52_Cylinder_Down, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                var coditionStep = GetUnitStep((int)eStep.Step_12_Cylinder_Down);
                return (coditionStep.CheckCondition());
            },
            ExecuteAction = () =>
            {
                BaseCtrl.Instance.IndexPusherCtrl.SetPusherDown();
                return true;
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsPusherDown(100);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_I_Pusher2Dn);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_53_Servo_X_Unload_End;
            }
        });
        actionUnitStep.Add((int)eStep.Step_53_Servo_X_Unload_End, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if(!BaseCtrl.Instance.UnloadElevatorCtrl.IsReady())
                {
                    //SetAlarm("TODO");
                    return false;
                }    

                return true;
            },
            ExecuteAction = () =>
            {
                BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.UNLOAD_END);
                return true;
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.UNLOAD_END);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_54_Servo_Cylinder_Up;
            }
        });
        actionUnitStep.Add((int)eStep.Step_54_Servo_Cylinder_Up, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                return true;
            },
            ExecuteAction = () =>
            {
                BaseCtrl.Instance.IndexPusherCtrl.SetPusherUp();
                return true;
            },
            IsStateOK = () =>
            {
                SetStripLocation(eStripLocation.UNLOAD);
                return BaseCtrl.Instance.IndexPusherCtrl.IsPusherUp();
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_I_Pusher2Up);
            },
            NextStep = () =>
            {
                return (int)eStep.Step_55_Servo_Servo_X_Ready;
            }
        });
        actionUnitStep.Add((int)eStep.Step_55_Servo_Servo_X_Ready, new UnitStep(5000)
        {
            CheckCondition = () =>
            {
                if(!BaseCtrl.Instance.PlasmaCtrl.IsChamberUp())
                {
                    SetAlarm((int)eErrCode.Ecode_I_ChamberUp_Pm1);
                    return false;
                }    
                return true;
            },
            ExecuteAction = () =>
            {
                BaseCtrl.Instance.IndexPusherCtrl.StartXServo(ePositionIndexPusher.READY);
                return true;
            },
            IsStateOK = () =>
            {
                return BaseCtrl.Instance.IndexPusherCtrl.IsXMotorPos(ePositionIndexPusher.READY);
            },
            ActionTimeout = () =>
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_TimeOut);
            },
            NextStep = () =>
            {
                return (int)eStep.END;
            }
        });
    }

    private void UnitStepCheck()
    {
        if (unitStep == null) return;
        if (!unitStep.IsComplete()) return;

        NextStep((eStep)unitStep.NextStep());
    }

    private void CheckServoLimit()
    {
        if (!GetWork()) return;

        //====== Check Servo Limit =======
        if (BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.IsNegativeLimit())
        {
            SetAlarm((int)eErrCode.Ecode_IndexPush_X_MinLimit);
            return;
        }
        if (BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.IsPositiveLimit())
        {
            SetAlarm((int)eErrCode.Ecode_IndexPush_X_MaxLimit);
            return;
        }

        if(BaseCtrl.Instance.IndexPusherCtrl.IsServoErr())
        {
            if(BaseCtrl.Instance.IndexPusherCtrl.IndexPusherAxis.IsHomed())
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_NotHome);
            }
            else
            {
                SetAlarm((int)eErrCode.Ecode_IndexPush_X_AmpAlarm);
            }
            return;
        }    
    }

    private void CheckServoEnable()
    {
        if (!GetWork()) return;

        BaseCtrl.Instance.IndexPusherCtrl.EnableServo(true);
    }

    private bool StripIsLoadLocation()
    {
        return StripManager.Instance.GetStripLocation(eStripLocation.LOADED);
    }

    private void SetStripLocation(eStripLocation location)
    {
        StripManager.Instance.SetStripLocation(location);
    }

    enum eStep
    {
        IDLE,

        START,

        STEP_1_CHECK_READY,

        STEP_2_DIVERGENCE,

        STEP_10_LOAD_STRIP_TO_CHAMBER,
            Step_11_Servo_X_Load_Start,
            Step_12_Cylinder_Down,
            Step_13_Servo_X_Load_End,
            Step_14_Servo_X_In_Edge,
            Step_15_Cylinder_Up,
            Step_16_Loading_Complete,

        STEP_50_LOAD_STRIP_TO_UNLOAD,
            Step_51_Servo_X_Unload_Start,
            Step_52_Cylinder_Down,
            Step_53_Servo_X_Unload_End,
            Step_54_Servo_Cylinder_Up,
            Step_55_Servo_Servo_X_Ready,

        END,
    }
}
