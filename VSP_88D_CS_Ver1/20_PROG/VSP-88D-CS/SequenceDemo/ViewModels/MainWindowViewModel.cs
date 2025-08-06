using SequenceDemo.Sequences.Constants;
using SequenceEngine.Constants;
using SequenceEngine.Manager;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;

namespace SequenceDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private SequenceManager _sequenceManager;

    private eSeqState _sequenceState = eSeqState.STOP;
    public eSeqState SequenceState
    {
        get => _sequenceState;
        set => SetProperty(ref _sequenceState, value);
    }

    private ObservableCollection<StepItem> _steps = new();
    public ObservableCollection<StepItem> Steps
    {
        get => _steps;
        set => SetProperty(ref _steps, value);
    }

    private int _currentStepIndex;
    public int CurrentStepIndex
    {
        get => _currentStepIndex;
        set => SetProperty(ref _currentStepIndex, value);
    }

    //public bool IsCurrent => Step == CurrentStep;


    public ICommand StartSequenceCommand { get; }
    public ICommand StopSequenceCommand { get; }
    public ICommand CancelSequenceCommand { get; }
    public ICommand InitializSequenceCommand { get; }

    public MainWindowViewModel(SequenceManager sequenceManager)
    {
        _sequenceManager = sequenceManager;

        StartSequenceCommand = new RelayCommand(Start);
        StopSequenceCommand = new RelayCommand(Stop);
        CancelSequenceCommand = new RelayCommand(Cancel);
        InitializSequenceCommand = new RelayCommand(Initializ);

        _sequenceManager.StateChanged += SequenceStateChanged;
        _sequenceManager.GetModule((int)eSequenceModule.IndexPusher).StepChanged += StepChanged;

        foreach (eStepIndexPusher step in Enum.GetValues(typeof(eStepIndexPusher)))
        {
            string name = step.ToString();

            Steps.Add(new StepItem { DisplayName = name, Step = (int)step });
        }
    }

    private void Start()
    {
        _sequenceManager?.Start();
    }

    private void Stop()
    {
        _sequenceManager?.Stop();
    }

    private void Initializ()
    {
        _sequenceManager.Initial();
    }

    private void Cancel()
    {
        _sequenceManager?.Cancel();
    }

    private void SequenceStateChanged(eSeqState seqState)
    {
        SequenceState = seqState;
    }

    private void StepChanged(int  step)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CurrentStepIndex = step;
        });

    }
}

public class StepItem
{
    public string DisplayName { get; set; }
    public int Step { get; set; }

    public bool IsCurrent { get; set; }
    public bool IsDone { get; set; }
    public bool IsError { get; set; }
}
