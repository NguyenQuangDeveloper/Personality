using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Device;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.Views.Setting.Sub;
using VSP_COMMON;
using VSP_COMMON.VS_TIMER;
using static VS_UTILS;

namespace VSP_88D_CS.Sequence.Profiles
{
    public class uSwLampProc : VS_THREAD
    {
        #region Property
        private bool m_bToggle;

        private CVS_TIMER m_BuzzOffTimer;
        private CVS_TIMER m_ToggleTimer;
        private CVS_TIMER m_DoorAlarmSkipResetTimer;

        private enum eIonizerPort
        {
            LOAD = 0,
            UNLD,
            MAX
        };
        private List<bool> m_bIonizerAlarmOn;
        bool m_bShouldBeIonizerOn;

        private bool m_bReset;  // PBS
        public bool GetReset() => m_bReset;
        public void SetReset(bool bVal) => m_bReset = bVal;

        private bool m_bDoorErr;

        public bool GetDoorErr() => m_bDoorErr;
        public void SetDoorErr(bool bVal) => m_bDoorErr = bVal;

        private bool m_bSafetyErr;
        public bool GetSafetyErr() => m_bSafetyErr;
        public void SetSafetyErr(bool bVal) => m_bSafetyErr = bVal;

        private bool m_bAirLow;
        public bool GetAirLow() => m_bAirLow;
        public void SetAirLow(bool bVal) => m_bAirLow = bVal;

        private bool m_bAirVacLow;
        private bool m_bN2Low;
        private bool m_bNegativePhaseErr;
        private bool m_bGenPwrOverload;
        private bool m_bServoPwrOverload;
        private bool m_bFfuAlarm;
        private bool m_bElecPnlOverHeat;

        private bool m_bGas1Low;
        private bool m_bGas2Low;
        private bool m_bGas3Low;
        private bool m_bGas4Low;

        private bool m_bBuzzTimeDone;
        public void SetBuzzTimeDone(bool bVal) => m_bBuzzTimeDone = bVal;

        private List<bool> m_bRfGenCommErr = new List<bool>((int)ePM.MAX);
        private bool m_bLoadConvSafetyErr;
        private bool m_bUnldConvSafetyErr;

        private bool m_bPaused;

        #endregion

        private VS_GLOBAL_DATA _globalData;
        private VS_SEQUENCE_MANAGER SystemSeq = VS_SEQUENCE_MANAGER.Instance;
        private readonly IVSIOController _ioCtrl;
        private readonly uDoorLockUnlockProc _door_proc;
        private IGlobalSystemOption _sysOption;

        public uSwLampProc(int nIdx, IVSIOController ioControl, IGlobalSystemOption globalSystemOption, uDoorLockUnlockProc door_proc) : base(nIdx)
        {
            _globalData = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
            m_strLogHead = "SW_LAMP";
            _ioCtrl = ioControl;
            _door_proc = door_proc;
            _sysOption = globalSystemOption;
            DevInit();
            m_bToggle = false;
            RefreshTowerSetVal();
            m_bPaused = false;
        }
        ~uSwLampProc()  // 
        {
            ServoAirOn(false);
            _globalData.SetIonizerOffReq(true);
            _ioCtrl.SingleSolOnOff(eDO.O_TowerGreen, false);
            _ioCtrl.SingleSolOnOff(eDO.O_TowerRed, false);
            _ioCtrl.SingleSolOnOff(eDO.O_TowerYellow, false);
            _ioCtrl.SingleSolOnOff(eDO.O_Buzzer, false);
            if (_sysOption.SystemItems.GetUseIonizer())
            {
                IonizerOnOff(false);
            }

        }

        protected override void DevInit()
        {
            m_BuzzOffTimer.Reset();
            ServoAirOn(true);
        }
        public void AllOutOff()
        {

        }
        public override void FindStep()
        {

        }
        protected override void ResetSeq()// 모든 변수 초기화
        {

        }

        public void ResetErrFlags()
        {
            m_bReset = false;   // PBS
            m_bDoorErr = false;
            m_bSafetyErr = false;
            m_bAirLow = false;
            m_bN2Low = false;
            m_bNegativePhaseErr = false;

            m_bGas1Low = false;
            m_bGas2Low = false;
            m_bGas3Low = false;

            m_bGenPwrOverload = false;
            m_bServoPwrOverload = false;
            m_bBuzzTimeDone = false;

            m_bFfuAlarm = false;
            m_bElecPnlOverHeat = false;

            m_bLoadConvSafetyErr = false;
            m_bUnldConvSafetyErr = false;

            for (int i = 0; i < (int)eIonizerPort.MAX; i++)
            {
                m_bIonizerAlarmOn[i] = false;
            }
            for (int i = (int)ePM.BTM; i < GetMaxPmCount(); i++)
            {
                m_bRfGenCommErr[i] = false;
                //GeneratorVal[i].m_nCommErrCnt = 0;
            }

            _globalData.SetErrFlag(false);
            if (_globalData.GetEmgOn())
                _globalData.SetEmgOn(false);
        }
        public void MonSafety()   // 초기화 화면에서 볼 수 있게
        {
            CheckSafetyInput(ref m_bAirLow, eDI.I_MainAirPressure, false, 100, eErrCode.Ecode_I_MainAirPressure);
            if (_sysOption.SystemItems.GetGasUse((int)eMFCType.MFC_1))
                CheckSafetyInput(ref m_bGas1Low, eDI.I_Gas1Pressure, false, 100, eErrCode.Ecode_I_Gas1Pressure);
            if (_sysOption.SystemItems.GetGasUse((int)eMFCType.MFC_2))
                CheckSafetyInput(ref m_bGas2Low, eDI.I_Gas2Pressure, false, 100, eErrCode.Ecode_I_Gas2Pressure);
            if (_sysOption.SystemItems.GetGasUse((int)eMFCType.MFC_3))
                CheckSafetyInput(ref m_bGas3Low, eDI.I_Gas3Pressure, false, 100, eErrCode.Ecode_I_Gas3Pressure);
            if (_sysOption.SystemItems.GetGasUse((int)eMFCType.MFC_4))
                CheckSafetyInput(ref m_bGas4Low, eDI.I_Gas4Pressure, false, 100, eErrCode.Ecode_I_Gas4Pressure);
            if (_globalData.GetAutoRun() || SystemSeq.AnyOneInitializing() || _door_proc.IsStartReserved())
            {
                if (_sysOption.SystemItems.GetSkipDoorAlarm() && _door_proc.IsDoorLocked(100) || !_door_proc.IsDoorLockUse())
                {
                    _door_proc.ResetDoorLockTimer();
                    CheckSafetyInput(ref m_bDoorErr, eDI.I_DoorFrontCenter, false, 10, eErrCode.Ecode_I_DoorFrontCenter);
                    CheckSafetyInput(ref m_bDoorErr, eDI.I_DoorFrontLeft, false, 10, eErrCode.Ecode_I_DoorFrontLeft);
                    CheckSafetyInput(ref m_bDoorErr, eDI.I_DoorFrontRight, false, 10, eErrCode.Ecode_I_DoorFrontRight);
                    CheckSafetyInput(ref m_bDoorErr, eDI.I_DoorLeft, false, 10, eErrCode.Ecode_I_DoorLeft);
                    CheckSafetyInput(ref m_bDoorErr, eDI.I_DoorRight, false, 10, eErrCode.Ecode_I_DoorRight);

                }
            }
            else
            {
                if (SystemSeq.IsInManualFunction() || _door_proc.IsPMManualMode())
                {
                    _door_proc.IsDoorCloseAll();
                }
            }
        }
        public void MonBuzzerOffTimer()
        {
            int nSetTime = _sysOption.SystemItems.GetBuzzerOffTime() * 1000;

            if (_ioCtrl.IsSolenoidOn(eDO.O_Buzzer))
            {
                if (!m_BuzzOffTimer.IsStarted())
                {
                    m_bBuzzTimeDone = false;
                    m_BuzzOffTimer.Start();
                }
                else
                {
                    int nPvTime = (int)m_BuzzOffTimer.GetElapsed();
                    if (nPvTime >= nSetTime)
                    {
                        m_bBuzzTimeDone = true;
                    }
                }
            }
            else
            {
                m_BuzzOffTimer.Reset();
            }

        }
        public void RefreshTowerSetVal()
        {

        }
        public void ServoAirOn(bool bOn)
        {
            if (bOn)
            {
                if (_ioCtrl.IsSolenoidOff(eDO.O_ServoPwrOn))
                {
                    _ioCtrl.SingleSolOnOff(eDO.O_ServoPwrOn, true);
                }
                //		O_ChillerPwr->On();
                //		O_MainAirOn->On();
            }
            else
            {
                _ioCtrl.SingleSolOnOff(eDO.O_ServoPwrOn, false);
                //		O_ChillerPwr->Off();
                //		O_MainAirOn->Off();
            }
            //TODO: Set Servo Power On
            //MotionManager->SetMtrSvrOn(SEL_ALL, bOn);
        }
        public bool IsIonizerOn(int nOnTime = 0)
        {
            return _sysOption.SystemItems.GetUseIonizer();
        }
        public void IonizerOnOff(bool bOn)
        {
            _ioCtrl.SingleSolOnOff(eDO.O_IonizerOn, bOn);
            _ioCtrl.SingleSolOnOff(eDO.O_IonizerOn2, bOn);
        }

        public bool IsFfuOn(int nOnTime = 0)
        {
            return false;
        }
        public void FfuOnOff(bool bOn)
        {

        }

        public bool IsLampOn(int nOnTime = 0)
        {
            throw new NotImplementedException();
        }
        public void LampOnOff(bool bOn)
        {

        }

        public void DoSafetyReset()
        {

        }
        public bool IsPMManualMode()
        {
            throw new NotImplementedException();
        }

        public void CheckOnlineRemote()
        {

        }

        public override void SendError(int nErrCode)
        {
            base.SendError(nErrCode);
        }
        protected override void Always()
        {
            base.Always();
            MonEmg();
            //if Monitor IO not showing up
            //{
            DoToggleTimer();
            DoTower();
            DoChiller();
            MonBuzzerOffTimer();
            DoFfu();
            DoLamp();
            if (_sysOption.SystemItems.GetUseIonizer())
            {
                DoIonizer();
            }
            //}
            MonSwitches();
            if (!_globalData.GetEmgOn())
            {
                MonSafety();
                MonRfGenComm();
                MonDoorAlarmSkipTimer();
            }
            CheckOnlineRemote();
        }
        protected override eReturnType AutoRun()
        {

            return eReturnType.BUSY;
        }

        protected override bool IsRunningCondition()
        {
            return true;
        }
        private void MonEmg()
        {
            if (!_globalData.GetEmgOn())
            {
                if (_ioCtrl.IsSensorOn(eDI.I_EmgSwFront) || _ioCtrl.IsSensorOn(eDI.I_EmgSwRear))
                {
                    _globalData.SetEmgOn(true);
                    if (_ioCtrl.IsSensorOn(eDI.I_EmgSwFront))
                    {
                        SendError((int)eErrCode.Ecode_I_EmgSwFront);
                    }
                    else if (_ioCtrl.IsSensorOn(eDI.I_EmgSwRear))
                    {
                        SendError((int)eErrCode.Ecode_I_EmgSwRear);
                    }
                }
                else
                {
                    _globalData.SetEmgOn(false);
                }
            }

        }
        private void MonRfGenComm()
        {
            throw new NotImplementedException();
        }

        private void CheckSafetyInput(ref bool bFlag, eDI Di, bool bOn, int nDly, eErrCode nErrNo)
        {

        }

        private void DoToggleTimer()
        {
            if (m_ToggleTimer.IsStarted())
            {
                if (m_ToggleTimer.GetElapsed() >= 300)
                {
                    m_ToggleTimer.Reset();
                    m_bToggle = !m_bToggle;
                    _ioCtrl.ToggleSingleSol(eDO.O_TowerGreen, m_bToggle);
                    _ioCtrl.ToggleSingleSol(eDO.O_TowerYellow, m_bToggle);
                    _ioCtrl.ToggleSingleSol(eDO.O_TowerRed, m_bToggle);
                }
            }
            else
            {
                m_ToggleTimer.Start();
            }
        }
        private void DoTower()
        {
            if (!_globalData.GetErrFlag())
            {
                if (_globalData.GetAutoRun())
                {
                    //PilotLampOnOff(eDO.O_TowerGreen, TowerLamp->GetTwrVal(TWR_GRN, TWR_AUTORUN));
                    //PilotLampOnOff(O_TowerYellow, TowerLamp->GetTwrVal(TWR_YEL, TWR_AUTORUN));
                    //PilotLampOnOff(O_TowerRed, TowerLamp->GetTwrVal(TWR_RED, TWR_AUTORUN));
                    //DoBuzzer(TWR_AUTORUN);

                    //			PilotLampOnOff(O_StartSwLamp, LAMP_ON);
                    //			PilotLampOnOff(O_StopSwLamp, LAMP_OFF);
                    //			PilotLampOnOff(O_ResetSwLamp, LAMP_OFF);
                }
                /* DONE : LOT */
                //else if(JobManager->GetLotStatus() == LOT_ENDED)
                //else if (JobManager->GetJobStatus() == LOT_ENDED)
                //{
                //    PilotLampOnOff(O_TowerGreen, TowerLamp->GetTwrVal(TWR_GRN, TWR_LOTEND));
                //    PilotLampOnOff(O_TowerYellow, TowerLamp->GetTwrVal(TWR_YEL, TWR_LOTEND));
                //    PilotLampOnOff(O_TowerRed, TowerLamp->GetTwrVal(TWR_RED, TWR_LOTEND));
                //    DoBuzzer(TWR_LOTEND);

                //    //			PilotLampOnOff(O_StartSwLamp, LAMP_OFF);
                //    //			PilotLampOnOff(O_StopSwLamp, LAMP_OFF);
                //    //			PilotLampOnOff(O_ResetSwLamp, LAMP_ON);
                //}
                //else //if(nStep == STEP_IDLE)
                //{
                //    PilotLampOnOff(O_TowerGreen, TowerLamp->GetTwrVal(TWR_GRN, TWR_STOP));
                //    PilotLampOnOff(O_TowerYellow, TowerLamp->GetTwrVal(TWR_YEL, TWR_STOP));
                //    PilotLampOnOff(O_TowerRed, TowerLamp->GetTwrVal(TWR_RED, TWR_STOP));
                //    DoBuzzer(TWR_STOP);

                //    //			PilotLampOnOff(O_StartSwLamp, LAMP_OFF);
                //    //			PilotLampOnOff(O_StopSwLamp, LAMP_OFF);
                //    //			PilotLampOnOff(O_ResetSwLamp, LAMP_ON);
                //}
            }
            //else
            //{
            //    PilotLampOnOff(O_TowerGreen, TowerLamp->GetTwrVal(TWR_GRN, TWR_ERR));
            //    PilotLampOnOff(O_TowerYellow, TowerLamp->GetTwrVal(TWR_YEL, TWR_ERR));
            //    PilotLampOnOff(O_TowerRed, TowerLamp->GetTwrVal(TWR_RED, TWR_ERR));
            //    DoBuzzer(TWR_ERR);

            //    //		PilotLampOnOff(O_StartSwLamp, LAMP_OFF);
            //    //		PilotLampOnOff(O_StopSwLamp, LAMP_ON);
            //    //		PilotLampOnOff(O_ResetSwLamp, LAMP_OFF);
            //}
        }

        private void PilotLampOnOff(eDO PilotLamp, int nMode)
        {

        }
        private void DoBuzzer(int nIdx)      // nIdx : BUZZ_OFF, BUZZ_ALARM, BUZZ_NOTICE
        {

        }
        private void DoChiller()
        {

        }
        private void DoLamp()
        {

        }

        void MonSwitches()
        {

        }

        private void DoIonizer()
        {

        }
        private void DoFfu()
        {

        }
        // Description	: ST micro Door Skip Reset Timer --------------------------
        private void MonDoorAlarmSkipTimer()
        {

        }

        private bool IsPassCondition()
        {
            if (m_bDoorErr || m_bSafetyErr || m_bAirLow || m_bN2Low || m_bNegativePhaseErr)
                return true;

            if (m_bGas1Low || m_bGas2Low || m_bGas3Low)
                return true;

            if (m_bGenPwrOverload || m_bServoPwrOverload || m_bFfuAlarm || m_bElecPnlOverHeat)
                return true;

            if (m_bLoadConvSafetyErr || m_bUnldConvSafetyErr)
                return true;

            for (int i = 0; i < (int)eIonizerPort.MAX; i++)
            {
                if (m_bIonizerAlarmOn[i])
                    return true;
            }

            m_bPaused = true;
            return false;
        }
    }
}
