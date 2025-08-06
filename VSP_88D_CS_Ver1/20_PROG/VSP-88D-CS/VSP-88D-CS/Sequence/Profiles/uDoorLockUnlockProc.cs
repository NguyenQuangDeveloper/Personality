using LoggerLib.Interfaces;
using SequenceEngine.Bases;
using SequenceEngine.Constants;
using SequenceEngine.Restore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Controller;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Device;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Helpers;
using VSP_88D_CS.Sequence.Constants;
using VSP_88D_CS.Sequence.Controllers;
using VSP_COMMON;
using VSP_COMMON.VS_TIMER;

namespace VSP_88D_CS.Sequence.Profiles
{
    public struct stDoorState
    {
        public string strDoorPos;
        public string strDoorState;
    }
    public class uDoorLockUnlockProc : BaseSequence
    {
        private ControllerManager _controller;
        private VS_GLOBAL_DATA _globalData;
        private IGlobalSystemOption _globalSystemOption;
        public override int ModuleId { get; set; } = (int)eSequenceModule.SeqDoorLock;
        public override string LogHead { get; set; } = "DOOR_LOCK";
        private ILoggingService _logger;

        private bool m_bDoorSolIsUnlockType;
        private bool m_bStartReserved;
        private bool m_bDoorSolOn;
        private BitArray m_bsDoorClose;
        private List<IDigitalIOData> m_LockSol = new List<IDigitalIOData>();
        private List<IDigitalIOData> m_DoorSensor = new List<IDigitalIOData>();
        private CVS_TIMER m_ReserveTimer;
        private CVS_TIMER m_DoorStateMonTimer;


        enum eDoorSensor
        {
            DOOR_FRONT_LEFT = 0,
            DOOR_FRONT_CENTER = 0,
            DOOR_FRONT_RIGHT = 0,
            DOOR_LEFT,
            DOOR_RIGHT,
            DOOR_SENSOR_MAX
        };

        public uDoorLockUnlockProc()
        {
            _controller = VSContainer.Instance.Resolve<ControllerManager>();
            _globalData = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
            _logger = VSContainer.Instance.Resolve<ILoggingService>();
            _globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            m_bDoorSolIsUnlockType = true;

            DevInit();
        }
        protected override void Restore(SequenceRestore state)
        {
            base.Restore(state);
        }

        public override void Stop()
        {
            base.Stop();
        }

        private void DevInit()
        {
            m_bDoorSolOn = false;
            m_bStartReserved = false;
            m_DoorSensor = new List<IDigitalIOData>()
            {
                DefinedDio.I_DoorFrontCenter,
                DefinedDio.I_DoorFrontLeft,
                DefinedDio.I_DoorFrontRight,
                DefinedDio.I_DoorLeft,
                DefinedDio.I_DoorRight
            };
            m_LockSol = new List<IDigitalIOData>
            {
                DefinedDio.O_DoorUnlockFrontCenter,
                DefinedDio.O_DoorUnlockFrontRight,
                DefinedDio.O_DoorUnlockLeft,
                DefinedDio.O_DoorUnlockRight
            };
            m_bsDoorClose = new BitArray((int)eDoorSensor.DOOR_SENSOR_MAX);
        }

        public void AllOutOff()
        {

        }

        public void FindStep()
        {
            m_ReserveTimer.Reset();
        }

        public void ReserveStart()
        {
            //if(_globalSystemOption.SystemItems.GetSkipDoorAlarm())
            m_bStartReserved = true;
            if (m_LockSol.Count == 0)
            {
                m_ReserveTimer.Reset();
                m_ReserveTimer.Start();
                NextStep(10);
            }
        }

        public void ReserveStop()
        {
            m_bStartReserved = false;
            if (m_LockSol.Count == 0)
            {
                NextStep(20);
            }
        }

        protected override void SetAlarm(int nErrorCode)
        {
            if(!_globalData.GetErrFlag())
            {
                _globalData.SetErrFlag(true);
                _logger.LogError(string.Format("{0}: Error No.{0} Step {0}", LogHead, nErrorCode, currentStep));
            }
            base.SetAlarm(nErrorCode);

        }
        public override void AlwaysRun()
        {
            base.AlwaysRun();
            DoDoorLock();
            MonDoorStateTimer();
        }

        public override eSequenceResult RunSequence()
        {
            if (!GetWork()) return eSequenceResult.NOT_READY;

            unitStep = GetUnitStep();
            switch ((eStep)currentStep)
            {
                case eStep.READY:
                    break;
                case eStep.DOOR_LOCK:
                    break;
                case eStep.DOOR_UNLOCK:
                    break;
                case eStep.END:
                    break;
            }
            return eSequenceResult.BUSY;
        }

        private void NextStep(eStep step = eStep.READY)
        {
            if (!GetWork()) return;
            base.NextStep((int)step);
            string log = string.Format("{0}", Enum.GetName(typeof(eStep), (eStep)currentStep));
            _logger.LogDebug(log);
        }

        public bool IsDoorLockUse()
        {
            return m_LockSol.Count > 0;
        }
        public bool IsStartReserved() => m_bStartReserved;
        public bool IsDoorUnlocked(int nTime = 0)
        {
            if (m_bDoorSolIsUnlockType)
            {
                return IsDoorLockSolOn(nTime);
            }
            else
            {
                return IsDoorLockSolOff(nTime);
            }
        }
        public bool IsDoorLocked(int nTime = 0)
        {
            if (m_bDoorSolIsUnlockType)
            {
                return IsDoorLockSolOff(nTime);
            }
            else
            {
                return IsDoorLockSolOn(nTime);
            }
        }
        public void ResetDoorLockTimer()
        {
            //for (int i = 0; i < m_LockSol.Count; i++)
            //{
            //    _ioCtrl.ResetSolenoidTimer(m_LockSol[i]);
            //}
        }
        private void DoorLockOnOff(bool bLock)
        {
            if (bLock)
            {
                if (m_bDoorSolIsUnlockType)
                    m_bDoorSolOn = false;
                else
                    m_bDoorSolOn = true;
            }
            else
            {
                if (m_bDoorSolIsUnlockType)
                    m_bDoorSolOn = true;
                else
                    m_bDoorSolOn = false;
            }
        }
        private bool IsDoorLockSolOn(int nTime = 0)
        {
            if (m_LockSol.Count == 0)
            {
                return false;
            }
            int LOCK_SOL_MAX = 10;
            BitArray baLockOn = new BitArray(LOCK_SOL_MAX);
            for (int i = 0; i < LOCK_SOL_MAX; i++)
            {
                if (i < m_LockSol.Count)
                {
                    baLockOn.Set(i, m_LockSol[i].IsOn());
                }
                else
                {
                    baLockOn.Set(i, true);
                }
            }
            if (!baLockOn.HasAllSet())
            {
                return false;
            }
            return m_LockSol[m_LockSol.Count - 1].IsOn(nTime);
        }
        private bool IsDoorLockSolOff(int nTime = 0)
        {
            if (m_LockSol.Count == 0)
                return false;

            const int LOCK_SOL_MAX = 10;
            BitArray bsLockOff = new BitArray(LOCK_SOL_MAX);

            for (int i = 0; i < LOCK_SOL_MAX; i++)
            {
                if (i < m_LockSol.Count)
                {
                    bsLockOff.Set(i, m_LockSol[i].IsOff());
                }
                else
                    bsLockOff.Set(i, true);
            }

            if (!bsLockOff.HasAllSet())
                return false;

            return m_LockSol[m_LockSol.Count - 1].IsOff(nTime);

        }
        private void DoDoorLock()
        {
            foreach (IDigitalIOData sol in m_LockSol)
            {
                if(m_bDoorSolOn)
                    sol.On();
                else
                    sol.Off();
            }
        }
        public bool IsDoorCloseAll(bool bWithAlarm = true)
        {
            if (_globalSystemOption.SystemItems.GetSkipDoorAlarm())
            {
                return true;
            } 

            if (DefinedDio.I_DoorFrontCenter.IsOff())
            {
                if (bWithAlarm)
                {
                    SetAlarm((int)eErrCode.Ecode_I_DoorFrontCenter);
                }
                return false;
            }

            if (DefinedDio.I_DoorFrontLeft.IsOff())
            {
                if (bWithAlarm)
                {
                    SetAlarm((int)eErrCode.Ecode_I_DoorFrontLeft);
                }
                return false;
            }


            if (DefinedDio.I_DoorFrontRight.IsOff())
            {
                if (bWithAlarm)
                {
                    SetAlarm((int)eErrCode.Ecode_I_DoorFrontRight);
                }
                return false;
            }
            if (DefinedDio.I_DoorLeft.IsOff())
            {
                if (bWithAlarm)
                {
                    SetAlarm((int)eErrCode.Ecode_I_DoorLeft);
                }
                return false;
            }
            if (DefinedDio.I_DoorRight.IsOff())
            {
                if (bWithAlarm)
                {
                    SetAlarm((int)eErrCode.Ecode_I_DoorRight);
                }
                return false;
            }

            return true;
        }
        public bool IsPMManualMode()
        {
            throw new NotImplementedException();
        }
        public void MonDoorStateTimer()
        {
            bool bRet = true;
            BitArray baDoorClose = new BitArray((int)eDoorSensor.DOOR_SENSOR_MAX);

            if (!m_DoorStateMonTimer.IsStarted())
                m_DoorStateMonTimer.Start();

            if (m_DoorStateMonTimer.GetElapsed() > 1000)
            {
                m_DoorStateMonTimer.Reset();
                for (int i = 0; i < (int)eDoorSensor.DOOR_SENSOR_MAX; i++)
                {
                    if (i < baDoorClose.Count - 1)
                    {
                        baDoorClose.Set(i, m_DoorSensor[i].IsOn());
                    }
                    else
                        baDoorClose.Set(i, true);
                }

                if (m_bsDoorClose != baDoorClose)
                {
                    for (int i = 0; i < (int)eDoorSensor.DOOR_SENSOR_MAX; i++)
                    {
                        if (i < baDoorClose.Count - 1)
                        {
                            if (m_bsDoorClose[i] != baDoorClose[i])
                            {
                                stDoorState DoorState = new stDoorState();

                                if ((int)eDoorSensor.DOOR_FRONT_CENTER == i)
                                {
                                    DoorState.strDoorPos = "FRONT_CENTER";
                                }
                                else if ((int)eDoorSensor.DOOR_FRONT_LEFT == i)
                                {
                                    DoorState.strDoorPos = "FRONT_LEFT";
                                }
                                else if ((int)eDoorSensor.DOOR_FRONT_RIGHT == i)
                                {
                                    DoorState.strDoorPos = "FRONT_RIGHT";
                                }
                                else if ((int)eDoorSensor.DOOR_LEFT == i)
                                {
                                    DoorState.strDoorPos = "LEFT";
                                }
                                else if ((int)eDoorSensor.DOOR_RIGHT == i)
                                {
                                    DoorState.strDoorPos = "RIGHT";
                                }

                                if (m_DoorSensor[i].IsOn())
                                {
                                    DoorState.strDoorState = "CLOSE";
                                }
                                else
                                {
                                    DoorState.strDoorState = "OPEN";
                                }
                                //EventProvider.Instance.Publish(new VS_CIM_SEND_EVENT_MSG { CEID = CEID_DOOR_STATE_CHANGE, param = DoorState });
                            }
                        }
                    }
                    m_bsDoorClose = baDoorClose;
                }
            }
        }
        enum eStep
        {
            READY,
            DOOR_LOCK = 10,
            DOOR_UNLOCK = 200,
            END = 100,
        }
    }
}
