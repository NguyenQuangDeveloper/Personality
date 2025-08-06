

namespace VSLibrary.Common.Log;

/// <summary>
/// Abstraction for log output functionality.
/// Provides a common interface for various logging systems (e.g., NLog, Serilog).
/// </summary>
public interface ILogProvider
{
    /// <summary>
    /// Writes an informational log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    void Info(string message);

    /// <summary>
    /// Writes a warning log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    void Warn(string message);

    /// <summary>
    /// Writes a debug log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    void Debug(string message);

    /// <summary>
    /// Writes an error log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    void Error(string message);

    /// <summary>
    /// Writes an error log message with exception details.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="ex">The exception information.</param>
    void Error(string message, Exception ex);
}

/// <summary>
/// Interface for log file writers.
/// Manages log output location, format, and options.
/// </summary>
public interface ILogWriter
{
    /// <summary>
    /// Sets the base file path (context) for log output.
    /// All subsequent <see cref="Write"/> calls use this path as the log file.
    /// </summary>
    /// <param name="relativePath">Relative path or filename (e.g., "System/VsLog.txt").</param>
    void SetContext(string relativePath);

    /// <summary>
    /// Writes a log message to the currently set context path.
    /// </summary>
    /// <param name="message">The log message to output.</param>
    /// <param name="type">The log level (default: Info).</param>
    void Write(string message, LogType type = LogType.Info);

    /// <summary>
    /// Writes a log message directly to the specified file path, ignoring context.
    /// </summary>
    /// <param name="relativePath">Log file path (e.g., "Error/Crash.txt").</param>
    /// <param name="message">The message to output.</param>
    /// <param name="type">The log level (default: Info).</param>
    void WriteDirect(string relativePath, string message, LogType type = LogType.Info);

    /// <summary>
    /// Sets the log writer options (directory, file size limit, format, etc.).
    /// </summary>
    /// <param name="options">Log option settings.</param>
    void SetOptions(LogOptions options);
}

/// <summary>
/// Interface for the log manager, acting as an entry point for the logging system.
/// Provides context-based log writing and initialization.
/// </summary>
public interface ILogManager
{
    /// <summary>
    /// Initializes the logging system and sets the default log file and directory.
    /// </summary>
    /// <param name="logFile">Default log file name (e.g., "VsLog.txt").</param>
    /// <param name="logDir">Root directory for log storage (e.g., @"D:\Logs").</param>
    void Initialize(string logFile = "VsLog.txt", string logDir = @"D:\Logs");

    /// <summary>
    /// Sets the base log file path (context).
    /// All subsequent <see cref="Write"/> calls use this path.
    /// </summary>
    /// <param name="path">Relative path or filename (e.g., "UI/Main.txt").</param>
    void SetContext(string path);

    /// <summary>
    /// Writes a log message using the currently set context path.
    /// </summary>
    /// <param name="message">The log message to output.</param>
    /// <param name="type">The log level (default: Info).</param>
    void Write(string message, LogType type = LogType.Info);

    /// <summary>
    /// Writes a log message directly to the specified file path, regardless of current context.
    /// </summary>
    /// <param name="relativePath">Log file path to use.</param>
    /// <param name="message">The log message.</param>
    /// <param name="type">The log level.</param>
    void WriteDirect(string relativePath, string message, LogType type = LogType.Info);
}
