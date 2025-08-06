using LoggerLib.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace LoggerLib.Executes;

public class SerilogLoggingService : ILoggingService
{
    private readonly ILogger _logger;

    public string Path { get; set; }
    public string Prefix { get; set; }

    public SerilogLoggingService()
    {
        _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
    }

    public void LogInfo(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => _logger.Information(message);

    public void LogWarning(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => _logger.Warning(message);

    public void LogError(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        Exception? ex = null
    ) => _logger.Error(ex, message);

    public void LogDebug(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => _logger.Debug(message);
}
