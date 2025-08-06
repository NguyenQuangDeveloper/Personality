using SequenceEngine.Bases;
using SequenceEngine.Constants;

namespace SequenceEngine.Manager;

public class SequenceManager
{
    private readonly List<ISequenceModule> _modules = new();
    private eRunMode _mode;
    private bool _isRunning = true;
    private bool _manualTrigger = false;
    private Thread _thread;
    private eSeqState _stateSequence = eSeqState.STOP;
    private eSeqState _prevState = eSeqState.STOP;
    private ISequenceModule? _currentManualModule;
    private eModeRunSequence _modeRun = eModeRunSequence.Synchronous;

    public Action<eSeqState> StateChanged { get; set; }

    public SequenceManager(eModeRunSequence runSequence = eModeRunSequence.Synchronous)
    {
        _thread = new Thread(MainLoop);
        _thread.IsBackground = true;
        _thread.Start();

        _modeRun = runSequence;
    }

    private void SetState(eSeqState state)
    {
        if (_stateSequence == state)
            return;

        _prevState = _stateSequence;
        _stateSequence = state;

        StateChanged?.Invoke(state);

        if (state == eSeqState.ERROR)
        {
            Stop();
        }
    }

    public eSeqState GetPreviousState()
    {
        return _prevState;
    }


    public eSeqState GetState()
    {
        return _stateSequence;
    }

    public void AddModule(List<ISequenceModule> modules)
    {
        if (modules == null) return;

        _modules.AddRange(modules);

        foreach (var module in modules)
        {
            module.Action += (status) => {
                SetState(status);
            };
        }    
    }

    public ISequenceModule GetModule(int moduleId)
    {
        return _modules[moduleId];
    }

    public void Initial()
    {
        if (_stateSequence == eSeqState.RUNNING && _stateSequence == eSeqState.PAUSE) return;

        Cancel();

        SetState(eSeqState.INITIALIZE);
    }

    public void ClearAlarm()
    {
        if (_stateSequence == eSeqState.RUNNING) return;

        foreach (var module in _modules)
        {
            module.ClearAlarm();
        }

        SetState(_prevState);
    }

    public void Start()
    {
        if (_stateSequence != eSeqState.READY && _stateSequence != eSeqState.PAUSE)
            return;

        SetState(eSeqState.RUNNING);

        _mode = eRunMode.Auto;

        foreach (var module in _modules)
        {
            module.Start();
        }
    }

    public void Stop()
    {
        SetState(eSeqState.PAUSE);

        _mode = eRunMode.Cycle;

        foreach (var module in _modules)
        {
            module.Stop();
        }
    }

    public void Cancel()
    {
        if (_stateSequence == eSeqState.RUNNING && _stateSequence == eSeqState.INITIALIZE) return;

        SetState(eSeqState.PAUSE);

        foreach (var cycle in _modules)
        {
            cycle.Cancel();
        }
    }

    public void Disposable()
    {
        SetState(eSeqState.STOP);
        _isRunning = false;
        foreach (var module in _modules)
        {
            module.Stop();
        }
        _thread.Join();
    }



    public void Run(eRunMode mode, int moduleId, int? manualStepIndex = null)
    {
        _mode = mode;

        if (_mode == eRunMode.Auto || _stateSequence == eSeqState.RUNNING) return;

        if ((_mode == eRunMode.Step || _mode == eRunMode.Cycle) && manualStepIndex.HasValue)
        {
            _currentManualModule = _modules.FirstOrDefault(c => c.ModuleId == moduleId);

            if (_currentManualModule == null) return;

            _currentManualModule.SetStep(manualStepIndex.Value);
            _manualTrigger = true;
        }
    }

    private void MainLoop()
    {
        while (_isRunning)
        {
            Thread.Sleep(10);
            ManualRun();
            AlwayRun();
            Initialize();
            AutoRun();
        }
    }

    private void ManualRun()
    {
        if (_mode != eRunMode.Step && _mode != eRunMode.Cycle)
            return;

        if (_manualTrigger && _currentManualModule != null)
        {
            var result = _currentManualModule.RunSequence();

            if (result == eSequenceResult.SUCCESS || result == eSequenceResult.FAILD)
            {
                _manualTrigger = false;
            }
        }
    }

    private void AutoRun()
    {
        if (_stateSequence != eSeqState.RUNNING)
            return;

        if (_mode != eRunMode.Auto)
            return;

        RunModules(module => module.RunSequence());
    }

    private void Initialize()
    {
        if (_stateSequence != eSeqState.INITIALIZE) return;

        if (_modules.All(m => m.IsInitialized))
        {
            SetState(eSeqState.READY);
            return;
        }

        foreach (var module in _modules)
            module.Stop();

        if (_modeRun == eModeRunSequence.Synchronous)
        {
            foreach (var module in _modules)
                module.Initialize();
        }
        else if(_modeRun == eModeRunSequence.Parallel)
        {
            Parallel.ForEach(_modules, module =>
            {
                if (!module.IsInitialized)
                {
                    module.Initialize();
                }
            });
        }    
    }

    private void AlwayRun()
    {
        RunModules(module => module.AlwaysRun());
    }

    private void RunModules(Action<ISequenceModule> action)
    {
        if (_modeRun == eModeRunSequence.Synchronous)
        {
            foreach (var module in _modules)
                action(module);
        }
        else if (_modeRun == eModeRunSequence.Parallel)
        {
            Parallel.ForEach(_modules, action);
        }
    }
}
