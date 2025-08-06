using Org.BouncyCastle.Math.Field;
using SequenceDemo.Sequences.Constants;
using SequenceEngine.Bases;
using SequenceEngine.Constants;
using SequenceEngine.Restore;
using System.Diagnostics;

namespace SequenceDemo.Sequences.IndexPusher;

public class SeqIndexPusher : BaseSequence
{
    enum eStep
    {
        IDLE                                    = -1,

        START                                   = 0,

        CHECK_PLASMA                            = 1,

        M_STEP10_LOAD_STRIP_TO_CHAMBER          = 10,
            S_Step11_Servo_X_Load_Start,
            S_Step12_Cylinder_Down,
            S_Step13_Servo_X_Load_End,
            S_Step14_Servo_X_In_Edge,
            S_Step15_Cylinder_Up,

        M_STEP50_LOAD_STRIP_TO_UNLOAD           = 50,
            S_Step51_Servo_X_Unload_Start,
            S_Step52_Cylinder_Down,
            S_Step53_Servo_X_Unload_End,
            S_Step54_Cylinder_Up,
            S_Step55_Servo_X_Ready,

        END
    }

    enum eServoCommand
    {
        READY,
        START_LOAD,
        END_LOAD,
        EDGE,
        START_UNLOAD,
        END_UNLOAD,
    }

    public override int ModuleId { get; set; } = (int)eSequenceModule.IndexPusher;
    public override string LogHead { get; set; } = "INDEX_PUSHER";
    private Dictionary<eStep, UnitStep> _actionUnitStep = new Dictionary<eStep, UnitStep>();
    private bool _chamberReady = false;
    private bool _chamberCompleted = false;


    public SeqIndexPusher()
    {
        InitializeSteps();
    }

    public override void Start()
    {
        base.Start();
        
    }

    public override void Stop()
    {
        base.Stop();
        // TODO: Logic Stop Servo.
        // ...
    }

    public override void ClearAlarm()
    {
        base.ClearAlarm();
        // TODO: Logic Clear Alarm for Servo
        // ...
    }

    public override void AlwaysRun()
    {
        // TODO: Implement the logic that should run continuously
    }

    public override bool Initialize()
    {
        if (IsInitialized) return true;

        // TODO: Implement initialization logic for the sequence

        return IsInitialized = true ;
    }

    public override eSequenceResult RunSequence()
    {
        if (!GetWork()) return eSequenceResult.NOT_READY;

        // Get the logic of the corresponding UnitStep
        if (_actionUnitStep.TryGetValue((eStep)currentStep, out UnitStep step))
        {
            // Start processing the UnitStep
            step.Start();
        }

        StepChanged?.Invoke(currentStep);

        switch ((eStep)currentStep)
        {
            case eStep.START:
                NextStep();
                break;

            case eStep.CHECK_PLASMA:

                if (!Delay(300))
                {
                    break;
                }

                if (ChamberIsReady())
                {
                    NextStep(eStep.M_STEP10_LOAD_STRIP_TO_CHAMBER);
                    break;
                }    

                break;

            case eStep.M_STEP10_LOAD_STRIP_TO_CHAMBER:

                if (!Delay(300))
                {
                    break;
                }

                if (ChamberIsReady() && IsChamberUp())
                {
                    NextStep();
                }    
     
                break;
            case eStep.S_Step15_Cylinder_Up:
            case eStep.S_Step14_Servo_X_In_Edge:
            case eStep.S_Step13_Servo_X_Load_End:
            case eStep.S_Step12_Cylinder_Down:
            case eStep.S_Step11_Servo_X_Load_Start:
                if (step == null) break;

                // When the UnitStep's condition is met and marked as complete,
                // move to the next step defined by NextStep()
                if (step.IsComplete())
                {
                    NextStep(step.NextStep());
                }    

                break;

            case eStep.M_STEP50_LOAD_STRIP_TO_UNLOAD:

                if (!Delay(300))
                {
                    break;
                }

                if (IsElevatorReady())
                {
                    NextStep();
                }    

                break;
            case eStep.S_Step55_Servo_X_Ready:
            case eStep.S_Step54_Cylinder_Up:
            case eStep.S_Step53_Servo_X_Unload_End:
            case eStep.S_Step52_Cylinder_Down:
            case eStep.S_Step51_Servo_X_Unload_Start:
                if (step == null) break;

                if (step.IsComplete())
                {
                    NextStep(step.NextStep());
                }

                break;

            case eStep.END:
                NextStep(eStep.START);
                break;
        }

        return eSequenceResult.BUSY;
    }

    protected override void Restore(SequenceRestore state)
    {
        if ((eStep)currentStep >= eStep.S_Step11_Servo_X_Load_Start && (eStep)currentStep <= eStep.S_Step15_Cylinder_Up)
        {
            currentStep = Math.Max(state.Step - 1, 0);
        }    
        else if ((eStep)currentStep >= eStep.S_Step51_Servo_X_Unload_Start && (eStep)currentStep <= eStep.S_Step55_Servo_X_Ready)
        {
            currentStep = Math.Max(state.Step - 1, 0);
        }   



        
        // TODO Logic for Restore (Cylinder, Servo)
    }

    protected override void Savestore(SequenceRestore state)
    {
        base.Savestore(state);
        // TODO Logic for Save (Cylinder, Servo)
    }

    private void NextStep(eStep step = eStep.IDLE)
    {
        if (!GetWork()) return;
        base.NextStep((int)step);
        string log = string.Format("{0}", Enum.GetName(typeof(eStep), (eStep)currentStep));
    }

    private void InitializeSteps()
    {
        _actionUnitStep.Add(eStep.S_Step11_Servo_X_Load_Start, new UnitStep(10000) // Time Out 10s, if timeout <= 0 -> not use timeout
        {
            CheckCondition = () => // Check the condition before performing 'ExecuteAction'
            {
                if(!ChamberIsReady())
                {
                    SetAlarm(" Chamber is not ready");
                    return false;
                }    
                return true;
            },

            ExecuteAction = () => // Perform the action: Servo Move, Cylinder UP/DOWN
            {
                if (!Delay(500)) return false;

                StartXServo(eServoCommand.START_LOAD);

                return true;
            },

            IsStateOK = () => // Check the state after ExecuteAction; only proceed if this returns true
            {
                return IsXMotorPos(eServoCommand.START_LOAD);
            },

            ActionTimeout = () => // This action is triggered if ExecuteAction exceeds the timeout duration
            {
                SetAlarm("Timeout: ");
            },

            NextStep = () => // When IsStateOK returns true, this determines the next step to proceed to
            {
                return (int)eStep.S_Step12_Cylinder_Down;
            }
        });

        _actionUnitStep.Add(eStep.S_Step12_Cylinder_Down, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if(!IsXMotorPos(eServoCommand.START_LOAD))
                {
                    SetAlarm("");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                SetCylinder(false);
                return true;
            },

            IsStateOK = () =>
            {
                return IsCylinderDown();
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Cylinder Down");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step13_Servo_X_Load_End;
            }
        });

        _actionUnitStep.Add(eStep.S_Step13_Servo_X_Load_End, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!ChamberIsReady())
                {
                    SetAlarm("");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                StartXServo(eServoCommand.END_LOAD);
                return true;
            },

            IsStateOK = () =>
            {
                return IsXMotorPos(eServoCommand.END_LOAD);
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Move Load End");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step14_Servo_X_In_Edge;
            }
        });

        _actionUnitStep.Add(eStep.S_Step14_Servo_X_In_Edge, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!ChamberIsReady())
                {
                    SetAlarm("");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                StartXServo(eServoCommand.EDGE);
                return true;
            },

            IsStateOK = () =>
            {
                return IsXMotorPos(eServoCommand.EDGE);
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Move EDGE");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step15_Cylinder_Up;
            }
        });

        _actionUnitStep.Add(eStep.S_Step15_Cylinder_Up, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!IsXMotorPos(eServoCommand.EDGE))
                {
                    SetAlarm("");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                SetCylinder(true);
                return true;
            },

            IsStateOK = () =>
            {
                return IsCylinderUp();
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: CylinderUp");
            },

            NextStep = () =>
            {
                return (int)eStep.M_STEP50_LOAD_STRIP_TO_UNLOAD;
            }
        });

        _actionUnitStep.Add(eStep.S_Step51_Servo_X_Unload_Start, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!IsChamberUp())
                {
                    SetAlarm("Chamber is not complete");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                SetCylinder(true);
                StartXServo(eServoCommand.START_UNLOAD);
                return true;    
            },

            IsStateOK = () =>
            {
                return IsCylinderUp() && IsXMotorPos(eServoCommand.START_UNLOAD);
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout:");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step52_Cylinder_Down;
            }
        });

        _actionUnitStep.Add(eStep.S_Step52_Cylinder_Down, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!IsXMotorPos(eServoCommand.START_UNLOAD))
                {
                    SetAlarm("Servo X position is unsafe.");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                SetCylinder(false);
                return true;
            },

            IsStateOK = () =>
            {
                return IsCylinderDown();
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: cylinder not down");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step53_Servo_X_Unload_End;
            }
        });

        _actionUnitStep.Add(eStep.S_Step53_Servo_X_Unload_End, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!IsElevatorReady())
                {
                    SetAlarm("Elevator not ready.");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                StartXServo(eServoCommand.END_UNLOAD);
                return true;
            },

            IsStateOK = () =>
            {
                return IsXMotorPos(eServoCommand.END_UNLOAD);
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Position END_UNLOAD");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step54_Cylinder_Up;
            }
        });

        _actionUnitStep.Add(eStep.S_Step54_Cylinder_Up, new UnitStep(10000)
        {
            CheckCondition = () =>
            {
                if (!IsXMotorPos(eServoCommand.END_UNLOAD))
                {
                    SetAlarm("Servo X position is unsafe.");
                    return false;
                }

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                SetCylinder(true);
                return true;
            },

            IsStateOK = () =>
            {
                return IsCylinderUp();
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Cylinder not up");
            },

            NextStep = () =>
            {
                return (int)eStep.S_Step55_Servo_X_Ready;
            }
        });

        _actionUnitStep.Add(eStep.S_Step55_Servo_X_Ready, new UnitStep(10000)
        {
            CheckCondition = () =>
            {

                if(!IsCylinderUp())
                {
                    SetAlarm("Cylinder Is Not Up");
                    return false;
                }    

                return true;
            },

            ExecuteAction = () =>
            {
                if (!Delay(500)) return false;
                StartXServo(eServoCommand.READY);
                return true;
            },

            IsStateOK = () =>
            {
                return IsXMotorPos(eServoCommand.READY);
            },

            ActionTimeout = () =>
            {
                SetAlarm("Timeout: Position READY");
            },

            NextStep = () =>
            {
                return (int)eStep.END;
            }
        });
    }

    private bool IsElevatorReady()
    {
        return true;
    }

    private bool IsChamberUp()
    {
        return true;
    }

    private bool ChamberIsReady()
    {
        return true;
    }

    private bool ChamberCompleted()
    {
        return false;
    }

    private void SetAlarm(string message)
    {
        Action?.Invoke(eSeqState.ERROR);
    }

    private void StartXServo(eServoCommand servoCommand)
    {

    }

    private bool IsXMotorPos(eServoCommand servoCommand)
    {
        return true;
    }

    private void SetCylinder(bool up)
    {

    }

    private bool IsCylinderUp()
    {
        return true;
    }

    private bool IsCylinderDown()
    {
        return true;
    }
}
