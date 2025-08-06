using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.Log;

/// <summary>
/// Static class that acts as the central entry point for log output.
/// Integrates and controls log flow using ILogProvider and ILogWriter.
/// </summary>
public static class LogManager 
{
    /// <summary>
    /// Proxy object for the log manager.
    /// Delegates logging operations via an ILogManager implementation (used with DI containers).
    /// </summary>
    private static ILogManager? _proxy;

    /// <summary>
    /// Optional DI container reference.
    /// </summary>
    private static IContainer? _container;

    /// <summary>
    /// Map of log providers, managed per context.
    /// </summary>
    private static readonly Dictionary<string, ILogProvider> _providerMap = new();

    /// <summary>
    /// Map of log writers, managed per context.
    /// </summary>
    private static readonly Dictionary<string, ILogWriter> _writerMap = new();

    /// <summary>
    /// Map of log options, managed per context.
    /// </summary>
    private static readonly Dictionary<string, LogOptions> _optionsMap = new();

    /// <summary>
    /// Lock object for synchronization.
    /// Prevents concurrency issues during log initialization and context changes.
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// Default log context key.
    /// Enables default logging even without explicit Initialize or SetContext calls.
    /// </summary>
    private const string DefaultContextKey = "VsLog.txt";

    /// <summary>
    /// Initializes the logging system.
    /// </summary>
    /// <param name="logFile">Default log file path (relative or absolute).</param>
    /// <param name="logDir">Log directory path.</param>
    public static void Initialize(string logFile = DefaultContextKey, string logDir = @"D:\Logs")
    {
        lock (_lock)
        {
            var options = new LogOptions
            {
                LogDirectory = logDir,
                MaxFileSizeMB = 5,
                EnableAutoZip = false,
                LogFormat = "[{type}] {time} > {message}"
            };

            var provider = new NLogProvider();
            var writer = new BaseLogWriter();
            writer.SetOptions(options);
            writer.SetContext(logFile);

            _providerMap[logFile] = provider;
            _writerMap[logFile] = writer;
            _optionsMap[logFile] = options;

            if (_proxy == null)
            {
                _proxy = new LogManagerProxy(logFile, logDir);
            }
        }
    }

    /// <summary>
    /// Sets the DI container and registers the log manager proxy.
    /// </summary>
    /// <param name="container">DI container instance.</param>
    public static void SetContainer(IContainer container)
    {
        _container = container;
        _proxy = new LogManagerProxy();
        _container.RegisterInstance<ILogManager>(_proxy);
    }

    /// <summary>
    /// Changes the current log context.
    /// </summary>
    /// <param name="relativePath">New log file path.</param>
    public static void SetContext(string relativePath)
    {
        _proxy?.SetContext(relativePath);
    }

    /// <summary>
    /// Sets the log writer and applies existing options.
    /// </summary>
    /// <param name="writer">ILogWriter implementation.</param>
    /// <param name="context">Target log context (file name).</param>
    public static void SetWriter(ILogWriter writer, string context = DefaultContextKey)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        _writerMap[context] = writer;
        if (_optionsMap.TryGetValue(context, out var options))
            writer.SetOptions(options);
    }

    /// <summary>
    /// Sets the log options for a given context.
    /// </summary>
    /// <param name="options">LogOptions instance.</param>
    /// <param name="context">Target log context (file name).</param>
    public static void SetLogOptions(LogOptions options, string context = DefaultContextKey)
    {
        _optionsMap[context] = options ?? new LogOptions();
        if (_writerMap.TryGetValue(context, out var writer))
            writer.SetOptions(_optionsMap[context]);
    }

    /// <summary>
    /// Writes a log message using the current context.
    /// </summary>
    /// <param name="message">Log message.</param>
    /// <param name="type">Log level.</param>
    public static void Write(string message, LogType type = LogType.Info)
    {
        _proxy?.Write(message, type);
    }

    /// <summary>
    /// Writes a log message directly to the specified log file.
    /// </summary>
    /// <param name="relativePath">Log file path.</param>
    /// <param name="message">Log message.</param>
    /// <param name="type">Log level.</param>
    public static void WriteDirect(string relativePath, string message, LogType type = LogType.Info)
    {
        _proxy?.WriteDirect(relativePath, message, type);
    }

    /// <summary>
    /// Writes an informational log message.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Info(string message)
    {
        if (_providerMap.TryGetValue(DefaultContextKey, out var provider))
            provider.Info(message);
    }

    /// <summary>
    /// Writes a warning log message.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Warn(string message)
    {
        if (_providerMap.TryGetValue(DefaultContextKey, out var provider))
            provider.Warn(message);
    }

    /// <summary>
    /// Writes a debug log message.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Debug(string message)
    {
        if (_providerMap.TryGetValue(DefaultContextKey, out var provider))
            provider.Debug(message);
    }

    /// <summary>
    /// Writes an error log message.
    /// </summary>
    /// <param name="message">Message to write.</param>
    public static void Error(string message)
    {
        if (_providerMap.TryGetValue(DefaultContextKey, out var provider))
            provider.Error(message);
    }

    /// <summary>
    /// Writes an error log message with exception details.
    /// </summary>
    /// <param name="message">Log message.</param>
    /// <param name="ex">Exception details.</param>
    public static void Error(string message, Exception ex)
    {
        if (_providerMap.TryGetValue(DefaultContextKey, out var provider))
            provider.Error(message, ex);
    }
}

/// <summary>
/// Instance-based implementation of the ILogManager interface.
/// Designed to maintain independent log contexts per thread or task.
/// </summary>
public class LogManagerProxy : ILogManager
{
    /// <summary>
    /// Lock object for synchronization.
    /// Prevents concurrency issues during log initialization and context changes.
    /// </summary>
    private readonly object _sync = new();

    /// <summary>
    /// Dictionary of log writers, managed per context (log file).
    /// </summary>
    private readonly Dictionary<string, ILogWriter> _writers = new();

    /// <summary>
    /// Dictionary of log options, managed per context.
    /// </summary>
    private readonly Dictionary<string, LogOptions> _options = new();

    /// <summary>
    /// Async-local storage for the current log context key.
    /// Maintains independent values per thread or async flow.
    /// </summary>
    private static readonly AsyncLocal<string> _currentKey = new();

    /// <summary>
    /// Returns the current context key.  
    /// If not set, returns the default key ("VsLog.txt").
    /// </summary>
    private string CurrentContext => _currentKey.Value ?? "VsLog.txt";


    /// <summary>
    /// Initializes a new instance of the <see cref="LogManagerProxy"/> class.
    /// </summary>
    /// <param name="logFile">Initial log file name (e.g., "System/Main.txt").</param>
    /// <param name="logDir">Log storage directory (e.g., @"D:\Logs").</param>
    public LogManagerProxy(string logFile = "VsLog.txt", string logDir = @"D:\Logs")
    {
        Initialize(logFile, logDir);
        _currentKey.Value = logFile;
    }

    /// <summary>
    /// Initializes the logging system and registers a new context.
    /// Reuses existing contexts or creates new ones as needed.
    /// </summary>
    /// <param name="logFile">Initial log file name.</param>
    /// <param name="logDir">Root directory for log storage.</param>
    public void Initialize(string logFile = "VsLog.txt", string logDir = @"D:\Logs")
    {
        lock (_sync)
        {
            if (_writers.ContainsKey(logFile)) return;

            var options = new LogOptions
            {
                LogDirectory = logDir,
                MaxFileSizeMB = 5,
                EnableAutoZip = false,
                LogFormat = "[{type}] {time} > {message}"
            };

            var writer = new BaseLogWriter();
            writer.SetOptions(options);
            writer.SetContext(logFile);

            _options[logFile] = options;
            _writers[logFile] = writer;
        }
    }

    /// <summary>
    /// Sets the log context (file path).
    /// All subsequent Write calls use this context.
    /// Automatically initializes the context if it does not exist.
    /// </summary>
    /// <param name="path">Context key or log file path.</param>
    public void SetContext(string path)
    {
        lock (_sync)
        {
            if (!_writers.ContainsKey(path))
            {
                Initialize(path);
            }

            _currentKey.Value = path;
        }
    }

    /// <summary>
    /// Writes a log message to the current context.
    /// </summary>
    /// <param name="message">Log message.</param>
    /// <param name="type">Log level (Info, Warn, Debug, Error, etc).</param>
    public void Write(string message, LogType type = LogType.Info)
    {
        lock (_sync)
        {
            if (_writers.TryGetValue(CurrentContext, out var writer))
            {
                writer.Write(message, type);
            }
        }
    }

    /// <summary>
    /// Writes a log message directly to the specified log file, regardless of the current context.
    /// Automatically initializes the context if not registered.
    /// </summary>
    /// <param name="relativePath">Log file path.</param>
    /// <param name="message">Log message.</param>
    /// <param name="type">Log level.</param>
    public void WriteDirect(string relativePath, string message, LogType type = LogType.Info)
    {
        lock (_sync)
        {
            if (!_writers.ContainsKey(relativePath))
            {
                Initialize(relativePath);
            }

            _writers[relativePath].WriteDirect(relativePath, message, type);
        }
    }
}