using System.Runtime.InteropServices;
using VSLibrary.Common.Log;

namespace VSLibrary.Threading;

/// <summary>
/// Standard VSLibrary thread base class.
/// All user-defined threads should inherit from this class.
/// </summary>
public abstract class VSThread : ThreadBase<VSThread>
{
    /// <summary>
    /// Implement common behaviors here if needed.
    /// </summary>
}

/// <summary>
/// Generic-based thread base class for VSLibrary.
/// All user-defined threads must inherit from this class and implement <see cref="RunProc"/>.
/// </summary>
/// <typeparam name="TSelf">Self type (CRTP pattern)</typeparam>
public abstract class ThreadBase<TSelf> : IThread where TSelf : ThreadBase<TSelf>
{
    /// <summary>
    /// Internal .NET thread instance used to execute the thread loop.
    /// </summary>
    private Thread _thread = null!;

    /// <summary>
    /// Indicates whether a stop has been requested for the thread.
    /// </summary>
    private bool _requestStop;

    /// <summary>
    /// Stores the number of currently active high-precision threads.
    /// Used to enable/disable system-wide high timer resolution as needed.
    /// </summary>
    private static int _activeHighThreadCount = 0;

    /// <summary>
    /// User-defined thread name. If not set, the class name is used.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Default log file context path used for logging output.
    /// </summary>
    private string? _defaultLogContext;

    /// <summary> 
    /// Gets the thread name. 
    /// </summary>
    public string Name => _name ?? typeof(TSelf).Name;

    /// <summary>
    /// Gets the log context path for this thread.
    /// </summary>
    public string DefaultLogContext => _defaultLogContext ?? typeof(TSelf).Name;

    /// <summary> 
    /// Indicates whether the thread is currently running. 
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Internal variable storing the current thread status.
    /// </summary>
    private ThreadStatus _status = ThreadStatus.Unknown;

    /// <summary> 
    /// Gets or sets the thread's execution priority. 
    /// </summary>
    public ThreadPriorityLevel Priority { get; set; } = ThreadPriorityLevel.Medium;

    /// <summary>
    /// Gets or sets the thread status.
    /// Changing this value also updates <see cref="LastStatus"/>.
    /// </summary>
    public ThreadStatus Status 
    {
        get => _status;
        set
        {
            LastStatus = _status;
            _status = value;
        }
    }

    /// <summary>
    /// The most recently recorded thread status, used for status change tracking.
    /// </summary> 
    public ThreadStatus LastStatus { get; private set; } = ThreadStatus.Unknown;

    /// <summary>
    /// Explicitly sets the thread's name from external code. Can be set only once.
    /// </summary>
    /// <param name="name">Thread name</param>
    /// <exception cref="InvalidOperationException">If the name has already been set</exception>
    public void SetName(string name)
    {
        if (_name != null)
            throw new InvalidOperationException($"[{Name}]은 이미 이름이 설정된 스레드입니다.");

        _name = name;
    }

    /// <summary>
    /// Sets the default log context path for this thread.
    /// This setting is automatically applied on every thread cycle.
    /// </summary>
    /// <param name="context">Log file path (e.g., "Threading/CommThread.txt")</param>
    public void SetDefaultLogContext(string context)
    {
        _defaultLogContext = context;
        LogManager.SetContext(context); // 초기 실행 시 즉시 반영
    }

    /// <summary>
    /// Starts the thread.
    /// </summary>
    public void Start()
    {
        if (IsRunning)
            return;

        _requestStop = false;
        _thread = new Thread(ThreadLoop)
        {
            IsBackground = false,
            Name = $"{_name}"
        };
        _thread.Start();
    }

    /// <summary>
    /// Requests the thread to stop.
    /// </summary>
    public void Stop()
    {
        Status = ThreadStatus.Stopped;
        _requestStop = true;
    }

    /// <summary>
    /// The internal thread loop.
    /// Controls the main execution cycle and handles thread lifecycle events.
    /// </summary>
    private void ThreadLoop()
    {
        IsRunning = true;
        PrepareHighPrecisionIfNeeded();

        OnStarted();

        try
        {
            while (!_requestStop)
            {                  
                ExecuteThreadCycle();
                Thread.Sleep(GetSleepDuration());
            }
        }
        catch (Exception ex)
        {
            OnException(ex);
        }
        finally
        {
            IsRunning = false;
            ReleaseHighPrecisionIfNeeded();
            OnStopped();
        }
    }

    /// <summary>
    /// Executes a single thread cycle, according to the current status.
    /// </summary>
    private void ExecuteThreadCycle()
    {
        switch (Status)
        {
            case ThreadStatus.Running:
                LogManager.SetContext(DefaultLogContext);
                RunProc();                   
                break;

            case ThreadStatus.Idle:
            case ThreadStatus.Paused:
            case ThreadStatus.Stopped:
            case ThreadStatus.Completed:
            case ThreadStatus.Error:
            case ThreadStatus.Unknown:
            default:
                Thread.Sleep(250);
                break;
        }
    }

    /// <summary>
    /// Returns the sleep duration (in milliseconds) for each thread cycle, 
    /// depending on the thread's priority.
    /// </summary>
    private int GetSleepDuration()
    {
        return Priority switch
        {
            ThreadPriorityLevel.High => 1,
            ThreadPriorityLevel.Medium => 5,
            ThreadPriorityLevel.Low => 10,
            _ => 2
        };
    }

    /// <summary>
    /// Requests 1ms timer resolution if this is the first high-priority thread.
    /// Ensures high-precision timing when needed (for example, in real-time or control tasks).
    /// </summary>
    private void PrepareHighPrecisionIfNeeded()
    {
        if (Priority == ThreadPriorityLevel.High &&
            Interlocked.Increment(ref _activeHighThreadCount) == 1)
        {
            TimeResolutionHelper.Enable1msResolution();
        }
    }

    /// <summary>
    /// Releases 1ms timer resolution when no more high-priority threads remain.
    /// Restores system timer resolution to default to avoid unnecessary CPU load.
    /// </summary>
    private void ReleaseHighPrecisionIfNeeded()
    {
        if (Priority == ThreadPriorityLevel.High &&
            Interlocked.Decrement(ref _activeHighThreadCount) == 0)
        {
            TimeResolutionHelper.Disable1msResolution();
        }
    }

    /// <summary>
    /// The main body of the thread loop.
    /// Must be implemented by derived classes to define per-cycle thread behavior.
    /// </summary>
    protected abstract void RunProc();

    /// <summary>
    /// Called when the thread starts.
    /// Override to implement custom start-up behavior.
    /// </summary>
    protected virtual void OnStarted(){}

    /// <summary>
    /// Called when the thread stops.
    /// Override to implement custom clean-up logic.
    /// </summary>
    protected virtual void OnStopped(){}

    /// <summary>
    /// Called when an exception is thrown during thread execution.
    /// Override to handle error logging, recovery, or notification.
    /// </summary>
    /// <param name="ex">Exception object thrown</param>
    protected virtual void OnException(Exception ex) { }

    /// <summary>
    /// Releases thread resources.
    /// The default implementation requests the thread to stop.
    /// Override for advanced clean-up if necessary.
    /// </summary>
    public virtual void Dispose()
    {
        Stop();
    }
}

/// <summary>
/// Utility class for setting CPU core affinity.
/// Allows pinning the current thread to a specific CPU core.
/// </summary>
internal static class CpuAffinityHelper
{
    /// <summary>
    /// Retrieves a handle to the current thread (Win32 API).
    /// </summary>
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentThread();

    /// <summary>
    /// Sets the processor affinity mask for the specified thread (Win32 API).
    /// </summary>
    /// <param name="hThread">A handle to the thread whose affinity mask is to be set.</param>
    /// <param name="dwThreadAffinityMask">The affinity mask to be set for the thread.</param>
    /// <returns>The previous affinity mask of the thread.</returns>
    [DllImport("kernel32.dll")] private static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

    /// <summary>
    /// Pins the current thread to the specified CPU core.
    /// </summary>
    /// <param name="coreIndex">Zero-based index of the CPU core to assign.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="coreIndex"/> is out of the range of available CPU cores.
    /// </exception>
    public static void SetAffinity(int coreIndex)
    {
        int processorCount = Environment.ProcessorCount;
        if (coreIndex < 0 || coreIndex >= processorCount)
            throw new ArgumentOutOfRangeException(nameof(coreIndex));

        UIntPtr mask = (UIntPtr)(1 << coreIndex);
        SetThreadAffinityMask(GetCurrentThread(), mask);
    }
}

/// <summary>
/// Utility class for setting high timer resolution (1ms) on Windows.
/// </summary>
internal static class TimeResolutionHelper
{
    /// <summary>
    /// Sets the global timer resolution to the specified period (WinMM API).
    /// </summary>
    /// <param name="uMilliseconds">The timer resolution in milliseconds.</param>
    /// <returns>Returns 0 if successful, otherwise an error code.</returns>
    [DllImport("winmm.dll", SetLastError = true)] private static extern uint timeBeginPeriod(uint uMilliseconds);

    /// <summary>
    /// Resets the global timer resolution to the previous value (WinMM API).
    /// </summary>
    /// <param name="uMilliseconds">The timer resolution in milliseconds to end.</param>
    /// <returns>Returns 0 if successful, otherwise an error code.</returns>
    [DllImport("winmm.dll", SetLastError = true)] private static extern uint timeEndPeriod(uint uMilliseconds);

    /// <summary>
    /// Enables 1ms global timer resolution for the system.
    /// Use this to improve timing accuracy for high-precision threads.
    /// </summary>
    public static void Enable1msResolution() => timeBeginPeriod(1);

    /// <summary>
    /// Disables 1ms global timer resolution and restores the default system setting.
    /// </summary>
    public static void Disable1msResolution() => timeEndPeriod(1);
}

/// <summary>
/// Factory class for creating threads and delegating their registration to an external registrar.
/// </summary>
public static class ThreadFactory
{
    /// <summary>
    /// Creates a new thread instance and registers it with the specified <paramref name="registrar"/>.
    /// </summary>
    /// <typeparam name="T">The type of thread to create. Must derive from <see cref="ThreadBase{T}"/> and have a public parameterless constructor.</typeparam>
    /// <param name="registrar">The thread registrar responsible for managing the thread's lifecycle.</param>
    /// <returns>The created thread instance.</returns>
    public static T Create<T>(IThreadRegistrar registrar) where T : ThreadBase<T>, new()
    {
        var thread = new T();
        registrar.Register(thread);
        return thread;
    }

    /// <summary>
    /// Creates a virtual thread (lightweight thread-like object) and registers it with the specified <paramref name="registrar"/>.
    /// </summary>
    /// <param name="name">The name of the virtual thread.</param>
    /// <param name="action">The callback action to execute on each cycle, receiving a cycle counter as the argument.</param>
    /// <param name="priority">The execution priority for the virtual thread.</param>
    /// <param name="interval">Execution interval in milliseconds between cycles.</param>
    /// <param name="logPath">Log file path. If null, defaults to "Threading/{name}.txt".</param>
    /// <param name="registrar">The thread registrar responsible for managing the virtual thread.</param>
    public static void CreateVirtual(
        string name,
        Action<int> action,
        ThreadPriorityLevel priority,
        int interval,
        string? logPath,
        IThreadRegistrar registrar)
    {
        var thread = new VirtualThread(name, action, priority, interval, logPath);
        registrar.Register(thread);
    }
}

/// <summary>
/// Virtual thread class that can be dynamically created and executed at runtime.
/// Unlike traditional user-defined threads, this class operates based on an action delegate
/// directly defined in code, allowing flexible thread behavior without custom class definitions.
/// </summary>
public sealed class VirtualThread : VSThread
{
    /// <summary>
    /// Name of the user-defined virtual thread.
    /// Used for logging and tracking purposes.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// User-defined action to be executed on each thread cycle.
    /// The action receives a loop counter as its parameter, representing the current cycle number.
    /// </summary>
    private readonly Action<int> _action;

    /// <summary>
    /// Execution interval in milliseconds between each thread cycle.
    /// If set to zero or less, the loop executes continuously without sleep.
    /// </summary>
    private readonly int _interval;

    /// <summary>
    /// Number of cycles executed so far.
    /// </summary>
    private int _count = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualThread"/> class.
    /// </summary>
    /// <param name="name">The thread name (for logging and traceability).</param>
    /// <param name="action">
    /// The action to execute on each loop.  
    /// Receives the current loop count as its parameter.
    /// </param>
    /// <param name="priority">Thread execution priority (default: Medium).</param>
    /// <param name="interval">
    /// Interval between each cycle in milliseconds.  
    /// If zero or less, the loop runs as fast as possible without sleep.
    /// </param>
    /// <param name="logPath">
    /// Optional log file path.  
    /// If null or empty, defaults to "Threading/{name}.txt".
    /// </param>
    public VirtualThread(string name, Action<int> action, ThreadPriorityLevel priority, int interval, string? logPath = null)
    {
        _name = name;
        _action = action;
        _interval = interval;

        SetName(name);
        Priority = priority;
        Status = ThreadStatus.Running;

        var context = string.IsNullOrWhiteSpace(logPath) ? $"Threading/{name}.txt" : logPath;
        SetDefaultLogContext(context);

        Start();
    }

    /// <summary>
    /// Main execution body of the thread.
    /// Invokes the user-defined action on each loop,
    /// and sleeps for the specified interval between cycles.
    /// </summary>
    protected override void RunProc()
    {
        _count++;
        _action?.Invoke(_count);
        if (_interval > 0)
            Thread.Sleep(_interval);
    }
}