namespace SequenceEngine.Restore;

public class SequenceRestore
{
    public int Step { get; set; }
    public Dictionary<string, double> ServoPositions { get; set; } = new();
    public Dictionary<string, bool> CylinderStates { get; set; } = new();
}
