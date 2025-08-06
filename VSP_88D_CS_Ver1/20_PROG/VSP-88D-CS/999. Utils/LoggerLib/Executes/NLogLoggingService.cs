using LoggerLib.Interfaces;
using NLog;
using System.Runtime.CompilerServices;

namespace LoggerLib.Executes;

public class NLogLoggingService : ILoggingService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public string Path { get; set; }
    public string Prefix { get; set; }

    public void LogInfo(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    )
    {
        var logEventInfo = new LogEventInfo(LogLevel.Info, "", message);
        logEventInfo.Properties["prefix"] = Prefix;
        var className = GetClassNameFromFile(file);
        logEventInfo.SetCallerInfo(className, member, file, line);
        _logger.Info(logEventInfo);
    }

    public void LogWarning(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    )
    {
        var logEventInfo = new LogEventInfo(LogLevel.Warn, "", message);
        logEventInfo.Properties["prefix"] = Prefix;
        var className = GetClassNameFromFile(file);
        logEventInfo.SetCallerInfo(className, member, file, line);
        _logger.Warn(logEventInfo);
    }

    public void LogError(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        Exception? ex = null
    )
    {
        var logEventInfo = new LogEventInfo(LogLevel.Error, "", message)
        {
            Exception = ex
        };
        var className = GetClassNameFromFile(file);
        logEventInfo.SetCallerInfo(className, member, file, line);
        logEventInfo.Properties["prefix"] = Prefix;
        _logger.Error(logEventInfo);
    }

    public void LogDebug(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    )
    {
        var logEventInfo = new LogEventInfo(LogLevel.Debug, "", message);
        logEventInfo.Properties["prefix"] = Prefix;
        var className = GetClassNameFromFile(file);
        logEventInfo.SetCallerInfo(className, member, file, line);
        _logger.Debug(logEventInfo);
    }

    private static string GetClassNameFromFile(string filePath)
    {
        return System.IO.Path.GetFileNameWithoutExtension(filePath);
    }
}
