using SequenceDemo.Sequences.Constants;
using SequenceEngine.Bases;
using SequenceEngine.Constants;
using SequenceEngine.Restore;

namespace SequenceDemo.Sequences.Plasma;

public class SeqPlasma : BaseSequence
{
    enum eStep
    {
        IDLE = -1,

        START = 0,

        END
    }

    public override int ModuleId { get;set; } = (int)eSequenceModule.Plasma;
    public override string LogHead { get; set; } = "PLASMA";
    public override eSequenceResult RunSequence()
    {
        if (!GetWork()) return eSequenceResult.NOT_READY;

        switch ((eStep)currentStep)
        {
            case eStep.START:
                break;



            case eStep.END:
                break;
        }

        return eSequenceResult.BUSY;
    }
}
