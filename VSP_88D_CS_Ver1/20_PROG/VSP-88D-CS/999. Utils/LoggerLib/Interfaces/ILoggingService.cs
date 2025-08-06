using LoggerLib.Types;
using System.Runtime.CompilerServices;

namespace LoggerLib.Interfaces;

public interface ILoggingService
{
    string Path { get; set; }
    string Prefix { get; set; }

    void LogInfo(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    );

    void LogWarning(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    );

    void LogError(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        Exception? ex = null);

    void LogDebug(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    );
}

public interface ILoggingServiceFactory
{
    ILoggingService Create(LoggingType? type);
}
