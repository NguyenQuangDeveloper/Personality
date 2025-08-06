using NLog;
using NLog.Config;
using NLog.Targets;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;

namespace VSLibrary.Common.Log;

/// <summary>
/// NLog-based logger implementation.
/// Provides unified log output via ILogProvider.
/// </summary>
public class NLogProvider : ILogProvider
{
    /// <summary>
    /// Static logger instance for this class (NLog).
    /// </summary>
    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Static constructor.
    /// If nlog.config does not exist, applies default NLog configuration for fallback.
    /// </summary>
    static NLogProvider()
    {
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog.config");

        if (File.Exists(configPath))
        {
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(configPath);
        }
        else
        {            
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("file")
            {
                FileName = "Logs/log_${shortdate}.txt",
                Layout = "${longdate} | ${level:uppercase=true} | ${message} ${exception:format=ToString}"
            };

            var consoleTarget = new ConsoleTarget("console")
            {
                Layout = "${longdate} | ${level:uppercase=true} | ${message}"
            };

            config.AddTarget(fileTarget);
            config.AddTarget(consoleTarget);

            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);

            NLog.LogManager.Configuration = config;
        }

        _logger = NLog.LogManager.GetCurrentClassLogger();
    }

    /// <summary>
    /// Writes an informational log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    public void Info(string message) => _logger.Info(message);

    /// <summary>
    /// Writes a warning log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    public void Warn(string message) => _logger.Warn(message);

    /// <summary>
    /// Writes a debug log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    public void Debug(string message) => _logger.Debug(message);

    /// <summary>
    /// Writes an error log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    public void Error(string message) => _logger.Error(message);

    /// <summary>
    /// Writes an error log message with exception details.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="ex">The exception object.</param>
    public void Error(string message, Exception ex) => _logger.Error(ex, message);
}

/// <summary>
/// Configuration class for log options.
/// Contains information such as log storage directory, file size limit, and output format.
/// </summary>
public class LogOptions
{
    /// <summary>
    /// Root directory for log storage.  
    /// Default is "Logs".
    /// </summary>
    public string LogDirectory { get; set; } = "Logs";

    /// <summary>
    /// Maximum file size (in MB) for a single log file.  
    /// If exceeded, log files are automatically rotated.
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 10;

    /// <summary>
    /// Whether to automatically compress previous log files into ZIP archives when the date changes.
    /// </summary>
    public bool EnableAutoZip { get; set; } = false;

    /// <summary>
    /// Log output format string.  
    /// Example: "[{type}] {time} &gt; {message}"
    /// </summary>
    public string LogFormat { get; set; } = "[{type}] {time} > {message}";
}

/// <summary>
/// Basic implementation of a file-based log writer.
/// Splits log files by date and index, and automatically rotates files when size limits are exceeded.
/// </summary>
public class BaseLogWriter : ILogWriter
{
    /// <summary>
    /// Lock object for synchronization in multithreaded environments.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Dictionary that tracks the current file index for each log file path.
    /// Used for file name rotation: [0000], [0001], etc.
    /// </summary>
    private readonly ConcurrentDictionary<string, int> _fileIndex = new();

    /// <summary>
    /// Relative path used for current log writing.
    /// Set via SetContext().
    /// </summary>
    private string _contextPath = "Default.log";

    /// <summary>
    /// Options for log output, including directory, format, and rotation criteria.
    /// </summary>
    private LogOptions _options = new();

    /// <summary>
    /// Sets the log output options.
    /// </summary>
    /// <param name="options">Log option settings.</param>
    public void SetOptions(LogOptions options)
    {
        _options = options ?? new LogOptions();
    }

    /// <summary>
    /// Sets the base relative path for log writing.
    /// Subsequent Write() calls will use this path as the log file target.
    /// </summary>
    /// <param name="relativePath">Example: "System/Boot.txt"</param>
    public void SetContext(string relativePath)
    {
        lock (_lock)
        {
            string dir = _options.LogDirectory ?? "Logs";
            _contextPath = Path.Combine(dir, relativePath ?? "Default.log");
        }
    }

    /// <summary>
    /// Writes a log message using the currently set context path.
    /// The file name is generated automatically by date and index.
    /// </summary>
    /// <param name="message">Log message to write.</param>
    /// <param name="type">Log level (Info, Warn, Debug, Error).</param>
    public void Write(string message, LogType type = LogType.Info)
    {
        lock (_lock)
        {
            WriteInternal(_contextPath, message, type);
        }
    }

    /// <summary>
    /// Writes a log message directly to the specified relative path, ignoring context.
    /// </summary>
    /// <param name="relativePath">Relative log path (e.g., "Error/Crash.txt").</param>
    /// <param name="message">Log message to write.</param>
    /// <param name="type">Log level.</param>
    public void WriteDirect(string relativePath, string message, LogType type = LogType.Info)
    {
        lock (_lock)
        {
            WriteInternal(relativePath, message, type);
        }
    }

    /// <summary>
    /// Internal implementation: actually writes data to the log file.
    /// Checks file size and determines whether to rotate files.
    /// </summary>
    /// <param name="relativePath">Relative log path.</param>
    /// <param name="message">Log message.</param>
    /// <param name="type">Log type (Info, Warn, etc).</param>
    private void WriteInternal(string relativePath, string message, LogType type)
    {
        try
        {
            lock (_lock)
            {
                string basePath = _options.LogDirectory ?? "Logs";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath, Path.GetDirectoryName(relativePath) ?? string.Empty);
                Directory.CreateDirectory(dir);

                string fileNameOnly = Path.GetFileNameWithoutExtension(relativePath);
                string extension = Path.GetExtension(relativePath);

                string baseFilePath = Path.Combine(dir, $"{fileNameOnly}_{date}");

                int index = _fileIndex.GetOrAdd(relativePath, 0);
                string indexedFile = $"{baseFilePath}[{index:D4}]{extension}";
                string logLine = FormatLine(type, message);

                if (File.Exists(indexedFile))
                {
                    var fileInfo = new FileInfo(indexedFile);
                    if (fileInfo.Length >= _options.MaxFileSizeMB * 1024 * 1024)
                    {
                        index++;
                        _fileIndex[relativePath] = index;
                        indexedFile = $"{baseFilePath}[{index:D4}]{extension}";
                    }
                }

                File.AppendAllText(indexedFile, logLine, Encoding.UTF8);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// Formats a log line according to the configured format.
    /// </summary>
    /// <param name="type">Log type.</param>
    /// <param name="message">Log message.</param>
    /// <returns>Formatted log line string.</returns>
    private string FormatLine(LogType type, string message)
    {
        string format = _options.LogFormat ?? "[{type}] {time} > {message}";
        return format
            .Replace("{type}", type.ToString().ToUpper())
            .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
            .Replace("{message}", message) + Environment.NewLine;
    }
}
