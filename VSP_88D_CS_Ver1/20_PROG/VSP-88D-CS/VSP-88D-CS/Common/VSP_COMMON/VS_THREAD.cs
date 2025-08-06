using LoggerLib.Interfaces;
using System.Diagnostics;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_COMMON.RECIPE_PARAM;
using VSP_COMMON.VS_TIMER;

namespace VSP_COMMON;

public class CVS_MOTOR;
public enum eRunType
{
    AUTO = 0,
    STEP,
    CYCLE
};

public enum eReturnType
{
    BUSY = 0,
    SUCCESS,
    PAUSE,
    FAIL
};

public abstract class VS_THREAD
{

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _task;
    private volatile bool _isPaused = false;

    #region Property
    protected int m_nIdx;
    protected uint m_nDelay;
    protected int m_MoveQueueDelay;
    protected string m_strLogHead;
    protected bool m_bStepLoggingOn;
    protected bool m_bUseStepLog;

    protected bool IsPass() { return (!m_bIsRun && m_nRunType == eRunType.AUTO); }
    protected virtual bool IsRunningCondition() { return true; }
    protected int m_nStep; protected void SetStep(int nVal) => m_nStep = nVal;
    protected eReturnType m_nReturn; protected void SetReturn(eReturnType nVal) => m_nReturn = nVal;
    protected eRunType m_nRunType; protected void SetRunType(eRunType nVal) => m_nRunType = nVal;
    protected int m_nPrevStep; protected void SetPrevStep(int nVal) => m_nPrevStep = nVal;
    protected int m_nInitStep; protected void SetInitStep(int nVal) => m_nInitStep = nVal;
    protected bool m_bIsRun; protected void SetRun(bool bVal) => m_bIsRun = bVal;
    protected bool m_bIsDryRun; protected void SetDryRun(bool bVal) => m_bIsDryRun = bVal;
    protected bool m_bIsReset; protected void SetReset(bool bVal) => m_bIsReset = bVal;
    protected bool m_bManualOn; protected void SetManualOn(bool bVal) => m_bManualOn = bVal;  // 수동 운전 중 표시 Flag
    protected bool m_bComplete;
    protected bool m_bReady;
    protected bool m_bInitOk;
    protected bool m_bInitializing;
    protected bool m_bInitStarted;
    protected bool m_bInitTimeOut;
    protected bool m_bOverloadDetected;
    protected bool m_bMoveAfterOverloadDetected;
    protected bool m_bErrorOn; // Error 발생 후 FindStep에서 사용하기 위함
    protected bool m_bStartedFlushMode;
    protected bool m_bWaitEndFlushMode;
    protected bool m_bPrimary;

    protected List<CVS_MOTOR> m_vSvrMtr = new List<CVS_MOTOR>();
    protected CWaitTimer m_WaitTimer = new CWaitTimer();
    protected CDelayTimer m_DelayTimer = new CDelayTimer();


    //Public member
    public bool m_bThreadRun;
    public bool m_bCancelInit;
    public string GetLogHead() => m_strLogHead;
    public eReturnType GetReturn() => m_nReturn;
    public eRunType GetRunType() => m_nRunType;
    public bool IsPassed() => IsPass();
    public bool IsInitOk() => m_bInitOk;
    public bool IsInitializing() => m_bInitializing;
    public bool IsInitStarted() => m_bInitStarted;
    public bool IsInitTimeOut() => m_bInitTimeOut;
    public int GetInitStep() => m_nInitStep;
    public bool IsRun() => m_bIsRun;
    public bool IsDryRun() => m_bIsDryRun;
    public bool IsReset() => m_bIsReset;
    public bool IsManualOn() => m_bManualOn;
    public void ResetManualOn() { SetRunType(eRunType.AUTO); m_bManualOn = false; }
    // Error 발생 후 FindStep에서 사용하기 위함
    public bool GetErrorOn() => m_bErrorOn;
    public void SetErrorOn(bool val) => m_bErrorOn = val;
    public bool IsComplete() => m_bComplete;
    public bool IsReady() => m_bReady;
    public void SetReady(bool bVal) => m_bReady = bVal;
    public void SetComplete(bool bVal) => m_bComplete = bVal;
    #endregion

    private VS_GLOBAL_DATA _globalData;
    private ILoggingService _logger;

    int SERVO_ALARM_START = 800;
    int SERVO_ALARM_KIND = 20;

    int FLUSH_SEQ = 900;

    public VS_THREAD(int nIdx, uint nDelay = 100)
    {
        //_logger = ;
        _globalData = VSContainer.Instance.Resolve<VS_GLOBAL_DATA>();
        _logger.Prefix = nameof(VS_THREAD);
        m_nIdx = nIdx;
        m_nDelay = nDelay;
        m_MoveQueueDelay = 50;
        m_bMoveAfterOverloadDetected = false;
        m_bStepLoggingOn = false;
        m_bPrimary = false;
        Initialize();
        DevInit();
    }

    protected virtual void DevInit() { }

    protected void Initialize()
    {
        m_nStep = 0;
        m_nReturn = eReturnType.BUSY;
        m_nRunType = eRunType.AUTO;

        m_bInitOk = false;
        m_bInitializing = false;
        m_bInitStarted = false;
        m_bInitTimeOut = false;
        m_nInitStep = 0;

        m_bIsRun = false;
        m_bIsDryRun = false;
        m_bIsReset = false;

        m_bComplete = false;
        m_bReady = false;

        m_bManualOn = false;
        // pusher들의 Overload 센서 감지시
        m_bOverloadDetected = false;

        m_bErrorOn = false;
        m_bStartedFlushMode = false;
        m_bWaitEndFlushMode = false;
        m_bThreadRun = false;
        m_bUseStepLog = false;
    }

    protected virtual void Always() { CheckRunMode(); } 
    
    protected void CheckActuator() { }

    protected virtual void ResetSeq() { }
    protected virtual void ResetData() { } // 모든 변수 초기화

    protected virtual eReturnType InitRun() { return eReturnType.BUSY; }
    protected virtual eReturnType AutoRun() { return eReturnType.BUSY; }

    public void Pause()
    {
        _isPaused = true;
        Debug.WriteLine($"{m_strLogHead} paused.");
    }

    public void Resume()
    {
        _isPaused = false;
        Debug.WriteLine($"{m_strLogHead} resumed.");
    }

    private async Task<eReturnType> RunProc(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Always();

            if (m_DelayTimer.IsRemainDelay())
                return eReturnType.BUSY;

            if (m_bInitializing && !m_bInitOk)
            {
                m_nReturn = InitRun();
            }
            else
            {
                if (IsRunningCondition())
                    m_nReturn = AutoRun();
                else
                    m_nReturn = eReturnType.BUSY;
            }

            switch (m_nRunType)
            {
                case eRunType.AUTO:
                    break;
                case eRunType.STEP:
                    if (m_nReturn != eReturnType.BUSY)
                    {
                        Stop();
                        return eReturnType.BUSY;
                    }
                    break;
                case eRunType.CYCLE:
                    if (m_nReturn >= eReturnType.PAUSE)
                    {
                        Stop();
                        return eReturnType.BUSY;
                    }
                    break;
            }

            return m_nReturn;
        }
        return m_nReturn;

    }
    public void StartThread()
    {
        if (_task != null && !_task.IsCompleted) return;

        _cts = new CancellationTokenSource();
        _task = Task.Run(() => RunProc(_cts.Token));
        m_bThreadRun = true;
        Debug.WriteLine($"{m_strLogHead} started.");
    }

    public void QuitThread()
    {
        _cts?.Cancel();
        m_bThreadRun = false;
        Debug.WriteLine($"{m_strLogHead} stopped.");
    }

    protected virtual void CheckRunMode()
    {
        if (m_bIsRun != _globalData.GetAutoRun())
        {
            m_bIsRun = _globalData.GetAutoRun();
            eProcessStep step = m_bIsRun ? eProcessStep.RUN : eProcessStep.STOP;
            EventProvider.Instance.Publish(new VS_PROC_MSG { Kind = eMsgKind.MODE_CHANGE, Step = step });
            if (m_bIsRun)
            {
                //LOG_PRINTF(L"CVS_THREAD", L"Thread %d : LogHead(%s) Step:(%d) m_bRead:(%d) m_bComplete(%d) Resume",
                //        m_dwThreadId, m_strLogHead.c_str(), m_nStep, m_bReady, m_bComplete);
                _logger.LogInfo($"Thread ({m_strLogHead}) Step:({m_nStep}) m_bReady:({m_bReady}) m_bComplete({m_bComplete}) Resume");
            }
            else
            {
                _logger.LogInfo($"Thread ({m_strLogHead}) Step:({m_nStep}) m_bReady:({m_bReady}) m_bComplete({m_bComplete}) Pause");
            }
        }
        m_bIsDryRun = _globalData.GetDryRun();
        DoFlushMode();
    }
    
    public virtual void StartInitial()
    {
        m_bInitOk = false;  //초기화 완료 된 것 까지 초기화 해버리는 현상 확인
        m_bInitializing = true;
        m_bInitStarted = false;
        m_bInitTimeOut = false;
        m_nInitStep = 0;
    }

    public virtual void CancelInitial()
    {
        //m_bCancelInit = true;
        //m_bInitOk = false;
        //m_bInitializing = false;
        //m_bInitStarted = false;
        //m_bInitTimeOut = false;
        //m_WaitTimer.Reset();

        //foreach (CVS_MOTOR _motor in m_vSvrMtr)
        //{
        //    if(_motor.IsHoming());
        //        _motor.SetOrgAbort(true);
        //}

        //Stop();
        throw new NotImplementedException();
    }
    
    protected void NextStep(int nStep = -1)
    {
        int nTemp = m_nStep;

        if (nStep < 0)
        {
            if (m_bInitializing && !m_bInitOk)
                m_nInitStep++;
            else
                m_nStep++;
        }
        else
        {
            if (m_bInitializing && !m_bInitOk)
                m_nInitStep = nStep;
            else
                m_nStep = nStep;
        }
        if (m_bUseStepLog)
        {
            _logger.LogInfo($"Thread ({m_strLogHead}): Step[{nTemp}->{m_nStep}]RunType[{m_nRunType}]");
        }
    }
    public int GetStepNo()
    {
        return (m_bInitializing && !m_bInitOk) ? m_nInitStep : m_nStep;
    }
    public void Stop()
    {
        if (m_bIsRun)
        {
            _globalData.SetAutoRun(false);
        }

        m_bManualOn = false;
        m_nRunType = eRunType.AUTO;
    }
    public void Step()
    {
        if (m_bIsRun)
            _globalData.SetAutoRun(false);

        Run(eRunType.STEP);
    }
    public void EStop()
    {
        Stop();
    }
    public void Cycle()
    {
        if (m_bIsRun)
            _globalData.SetAutoRun(false);

        Run(eRunType.CYCLE);
    }
    public void Run(eRunType nType)
    {

        m_nRunType = nType;
	        if (m_nRunType > eRunType.AUTO)
		        m_bManualOn = true;
    }
    public void Run(eRunType nType, int nStep)
    {
        m_nStep = nStep;
        Run(nType);
    }
    public void ResetProcStep()
    {
        Stop();
        m_nStep = 0;
        m_nPrevStep = 0;
        m_bStartedFlushMode = false;
        m_bWaitEndFlushMode = false;
        ResetSeq();
        ResetData();
    }
    public virtual void ResetError() { }
    public virtual void FindStep() { }
    public virtual string GetDebugInfo() => m_strLogHead; 
    public void SetEmergency()
    {
        //foreach( CVS_MOTOR _motor in m_vSvrMtr)
        //{
        //    _motor.SetEStop();
        //    _motor.ResetHomeDoneOk();
        //}
        throw new NotImplementedException();
    }

    public virtual void SendError(int nErrCode)
    {
        Stop();
        m_bErrorOn = true;
	        if(!_globalData.GetErrFlag())
	        {
            _globalData.SetErrFlag(true);
            _logger.LogInfo($"Thread ({m_strLogHead}) Error No.({nErrCode}) Step ({m_nStep})");
            EventProvider.Instance.Publish(new VS_ALARM_MSG { ErrCode = nErrCode, Kind = ALARM.SET });
        }
    }

    public void SetDelayTime(uint nDelay)
    {
        if (!m_DelayTimer.IsStarted())
        {
            m_DelayTimer.SetTimer(nDelay);
            m_DelayTimer.Start();
        }
    }

    public void SetWaitTimer(uint nWait)
    {
        if (nWait > 0)
        {
            m_WaitTimer.Reset();
            m_WaitTimer.SetTimer(nWait);
        }
    }

    public bool IsWaitTimeOut()
    {
        if(m_WaitTimer.IsWaitAlarm())
        {
            return true;
        }    
        return false;
    }
    
    public void ShowServoSoftMinAlarm(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 2;
        int nAlarmNo = nAlarmCode + (nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoSoftMaxAlarm(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 3;
        int nAlarmNo = nAlarmCode + (nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoTimeOutAlarm(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 8;
        int nAlarmNo = nAlarmCode + (nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowInvalidVelAccAlarm(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 11;
        int nAlarmNo = nAlarmCode + (nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoStartFailAlarm(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 9;
        int nAlarmNo = nAlarmCode + ((int)nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoHomeStartFail(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 6;
        int nAlarmNo = nAlarmCode + ((int)nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoHomeTimeOut(int nAxis)
    {
        int nAlarmCode = SERVO_ALARM_START + 7;
        int nAlarmNo = nAlarmCode + ((int)nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }
    public void ShowServoMotorAlarm(int nAxis, int byReason)
    {
        int nAlarmCode = SERVO_ALARM_START;

        switch ((eMtrAlarmType)byReason)
        {
            case eMtrAlarmType.NOT_HOME:
                nAlarmCode += 0;
                break;
            case eMtrAlarmType.AMP_ALARM:
                nAlarmCode += 1;
                break;
            case eMtrAlarmType.P_LMT_ON:
                nAlarmCode += 5;
                break;
            case eMtrAlarmType.N_LMT_ON:
                nAlarmCode += 4;
                break;
            case eMtrAlarmType.EMG_ON:
                nAlarmCode += 10;
                break;
        }

        int nAlarmNo = nAlarmCode + ((int)nAxis * SERVO_ALARM_KIND);
        SendError(nAlarmNo);
    }

    protected bool StartServoAxis(int nAxis, int nPosID, TMotionUnit MotionUnit, string strServo)
    {
        throw new NotImplementedException();
    }

    protected bool StartServoAxisOverloadDetected(Int32 nAxis, TMotionUnit MotionUnit, string strServo)
    {
        throw new NotImplementedException();
    }

    private bool StartJogServoAxis(Int32 nAxis, TMotionUnit MotionUnit, string strServo)
    {
        throw new NotImplementedException();
    }

    protected bool IsMotorPosAxis(Int32 nAxis, double dbPos, int nPosID = -1, bool bManual = false)
    {
        throw new NotImplementedException();
    }

    protected double GetMotorPosAxis(Int32 nAxis, int nPosID)
    {
        double dbPos = _globalData.GetRecipe().GetPosition((int)nAxis, nPosID);
        return dbPos;
    }

    protected bool IsMotorAlarm(int nAxis)
    {
        throw new NotImplementedException();
    }

    public void DoFlushMode()
    {
        if (_globalData.GetFlushRun() && !m_bStartedFlushMode)
        {
            m_bStartedFlushMode = true;
            SetPrevStep(m_nStep);
            NextStep(FLUSH_SEQ);
        }
    }

    public void SetPrimaryThread(bool bVal) => m_bPrimary = bVal;

    public void ResetMtrOverload()
    {
        m_bOverloadDetected = false;
    }

    protected virtual bool IsOverloadDetected(int nAxis) { return false; }

    protected virtual void SetStepAfterMoveAlarm(int nAxis) { }

    public virtual void StartJogMove(int nAxis, TMotionUnit val) {}

}
