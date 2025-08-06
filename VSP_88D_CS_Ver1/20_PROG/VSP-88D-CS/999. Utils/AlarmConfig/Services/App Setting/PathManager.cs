using System.IO;

namespace AlarmConfig.Services.App_Setting;

public sealed class PathManager
{
    private const string ImageAlarmFolderName = "Images";

    private static PathManager? _instance;
    public static PathManager Instance => _instance ??= new PathManager(Path.Combine(AppContext.BaseDirectory, "AlarmConfig"));

    private PathManager(string basePath)
    {
        PathConfig = basePath;
        PathAlarmImageStore = Path.Combine(PathConfig, ImageAlarmFolderName);

        EnsureCreated(PathConfig);
        EnsureCreated(PathAlarmImageStore);
    }

    public string PathConfig { get; }
    public string PathAlarmImageStore { get; }

    private static void EnsureCreated(string path) => Directory.CreateDirectory(path);

    public static void Init(string basePath)
    {
        _instance = new PathManager(basePath);
    }
}
