using SequenceEngine.Constants;
using SequenceEngine.Manager;
using SequenceEngine.Restore;
using System.Diagnostics;

namespace SequenceEngine.Bases;

public abstract class BaseSequence : ISequenceModule
{
    protected int currentStep;
    protected Stopwatch time;
    protected bool work;
    protected UnitStep unitStep;
    private readonly SequenceHistoryManager history;
    protected Dictionary<int, UnitStep> actionUnitStep = new Dictionary<int, UnitStep>();

    public abstract int ModuleId { get; set; }
    public abstract string LogHead { get; set; }
    public bool IsInitialized { get; set; }
    public Action<eSeqState> Action { get; set; }
    public Action<int> StepChanged { get; set; }

    public BaseSequence()
    {
        currentStep = 0;
        time = new Stopwatch();
        work = false;
        history = new SequenceHistoryManager();
    }

    public virtual bool Initialize() { return IsInitialized = true; }
    public abstract eSequenceResult RunSequence();
    public virtual void AlwaysRun() { }
    public virtual void ClearAlarm() { }
    protected virtual void Savestore(SequenceRestore state) { }
    protected virtual void Restore(SequenceRestore state) { }

    public virtual bool IsReady()
    {
        return !GetState();
    }

    public virtual void Start()
    {
        RestoreState(history.Undo());

        if (currentStep < 0)
            currentStep = 0;

        work = true;
        FlagManager.SetFlag(ModuleId, true);
    }

    public virtual void Stop()
    {
        work = false;
        FlagManager.SetFlag(ModuleId, false);

        var restore = SaveState();
        history.Save(restore);
    }

    public void Cancel()
    {
        work = false;
        currentStep = -1;
        history.Clear();
        FlagManager.SetFlag(ModuleId, false);
    }

    public void SetStep(int step)
    {
        currentStep = step;
    }

    public bool GetWork()
    {
        return work;
    }

    public bool GetState()
    {
        return FlagManager.IsRunning(ModuleId);
    }

    protected UnitStep GetUnitStep(int step = 0)
    {
        int stepId = currentStep;

        if (step == 0)
            stepId = step;

        if(actionUnitStep.TryGetValue(stepId, out UnitStep unit))
        {
            unit.Start();

            return unit;
        }    

        return null;
    }

    protected void NextStep(int step)
    {
        if (!work) return;

        SetTimer();

        if (step == -1)
            currentStep++;
        else
            currentStep = step;
    }

    protected void PreStep()
    {
        if (currentStep < 0)
            currentStep = -1;
        else
            currentStep--;
    }

    protected virtual void SetAlarm(int nErrorCode)
    {
        Stop();
        Action?.Invoke(eSeqState.ERROR);
    }

    protected bool Timeout(int milliseconds)
    {
        if (milliseconds <= 0) return false;

        return Delay(milliseconds);
    }

    protected bool Delay(int milliseconds)
    {
        return time.ElapsedMilliseconds > milliseconds;
    }

    private void SetTimer()
    {
        time.Restart();
    }

    private SequenceRestore SaveState()
    {
        SequenceRestore state = new SequenceRestore
        {
            Step = currentStep,
        };

        Savestore(state);

        return state;
    }

    public void RestoreState(SequenceRestore state)
    {
        if (state == null) return;

        currentStep = state.Step;

        Restore(state);
    }

    public int GetStep()
    {
        return currentStep;
    }
}
