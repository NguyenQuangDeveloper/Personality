using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Controllers;

public class StripManager
{
    private static readonly Lazy<StripManager> _instance = new Lazy<StripManager>(() => new StripManager());

    public static StripManager Instance => _instance.Value;

    public List<StripInfo> StripInfos { get; set; }


    private object _locker = new object();
    private StripManager()
    {
        StripInfos = new List<StripInfo>();
    }

    public void StartStrip()
    {
        lock (_locker)
        {
            StripInfos.Add(new StripInfo());
        }    
    }

    public void EndStrip()
    {
        lock (_locker)
        {
            if (StripInfos.Count > 0)
                StripInfos.RemoveAt(0);
        }    
    }

    public eStripLocation GetStripLocation()
    {
        var strip = StripManager.Instance.StripInfos
            .SelectMany(info => info.Strips)
            .LastOrDefault();

        return strip?.Location ?? eStripLocation.UNKNOW;
    }

    public bool GetStripLocation(eStripLocation desiredLocation)
    {
        var strips = StripManager.Instance.StripInfos
            .SelectMany(info => info.Strips)
            .ToList();

        for (int i = strips.Count - 1; i >= 0; i--)
        {
            if (strips[i].Location == desiredLocation)
            {
                return true;
            }
        }

        return false;
    }


    public void SetStripLocation(eStripLocation newLocation)
    {
        lock (_locker)
        {
            var strips = StripManager.Instance.StripInfos
                .SelectMany(info => info.Strips)
                .Where(s => s.StripState != eStripState.UNKNOW &&
                            s.StripState != eStripState.EMPTY)
                .ToList();

            for (int i = strips.Count - 1; i >= 0; i--)
            {
                var strip = strips[i];

                if (newLocation >= strip.Location)
                {
                    strip.Location = newLocation;
                    break;
                }
            }
        }
    }

    public eStripState GetStripState()
    {
        var strip = StripManager.Instance.StripInfos
            .SelectMany(info => info.Strips)
            .LastOrDefault(s =>
                s.StripState != eStripState.UNKNOW &&
                s.StripState != eStripState.EMPTY);

        return strip?.StripState ?? eStripState.UNKNOW;
    }

    public bool GetStripState(eStripState desiredState)
    {
        var strips = StripManager.Instance.StripInfos
            .SelectMany(info => info.Strips)
            .ToList();

        for (int i = strips.Count - 1; i >= 0; i--)
        {
            if (strips[i].StripState == desiredState)
            {
                return true;
            }
        }

        return false;
    }


    public void SetStripState(eStripState newState)
    {
        lock (_locker)
        {
            var strips = StripManager.Instance.StripInfos
                .SelectMany(info => info.Strips)
                .Where(s => s.StripState != eStripState.UNKNOW &&
                            s.StripState != eStripState.EMPTY)
                .ToList();

            for (int i = strips.Count - 1; i >= 0; i--)
            {
                var strip = strips[i];

                if (newState >= strip.StripState)
                {
                    strip.StripState = newState;
                    break;
                }
            }
        }
    }


}
