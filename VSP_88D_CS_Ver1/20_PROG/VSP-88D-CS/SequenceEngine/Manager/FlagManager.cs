namespace SequenceEngine.Manager;

public static class FlagManager
{
    private static readonly Dictionary<int, bool> _flags = new();

    public static void SetFlag(int id, bool state)
    {
        _flags[id] = state;
    }

    public static bool IsRunning(int id)
    {
        return _flags.TryGetValue(id, out var value) && value;
    }
}
