using SequenceEngine.Constants;

namespace SequenceEngine.Bases;

public interface ISequenceModule
{
    int ModuleId { get; set; }
    Action<eSeqState> Action { get; set; }
    Action<int> StepChanged { get; set; }
    void SetStep(int step);
    int GetStep();
    bool IsInitialized { get; set; }
    bool Initialize();
    bool IsReady();
    void Start();
    eSequenceResult RunSequence();
    void AlwaysRun();
    void Stop();
    void Cancel();
    void ClearAlarm();
}
