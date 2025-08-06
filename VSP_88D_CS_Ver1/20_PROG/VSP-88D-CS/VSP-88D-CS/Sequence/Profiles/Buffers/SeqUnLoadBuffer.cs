using SequenceEngine.Bases;
using SequenceEngine.Constants;
using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Profiles.Buffers;

public class SeqUnLoadBuffer : BaseSequence
{
    public override int ModuleId { get; set; } = (int)eSequenceModule.SeqUnLoadBuffer;
    public override string LogHead { get; set; } = "UNLD_BUFFER";
    public SeqUnLoadBuffer()
    {

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

        return IsInitialized = true;
    }

    protected override void SetAlarm(int nErrorCode)
    {
        base.SetAlarm(nErrorCode);
    }

    public override eSequenceResult RunSequence()
    {
        if (!GetWork()) return eSequenceResult.NOT_READY;

        unitStep = GetUnitStep();

        switch ((eStep)currentStep)
        {

        }

        return eSequenceResult.BUSY;
    }

    private void NextStep(eStep step = eStep.IDLE)
    {
        if (!GetWork()) return;
        base.NextStep((int)step);
        string log = string.Format("{0}", Enum.GetName(typeof(eStep), (eStep)currentStep));
    }

    enum eStep
    {
        IDLE,

        START,

        END,
    }
}
