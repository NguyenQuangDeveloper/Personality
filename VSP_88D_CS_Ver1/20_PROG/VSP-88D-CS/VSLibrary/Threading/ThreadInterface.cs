using System.Reflection;

namespace VSLibrary.Threading;

/// <summary>
/// Basic interface for common thread execution.
/// All user-defined threads must implement this interface.
/// </summary>
public interface IThread
{
    /// <summary>
    /// Defines the execution priority of the thread.
    /// </summary>
    ThreadPriorityLevel Priority { get; set; }

    /// <summary>
    /// Indicates whether the thread is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Unique name of the thread.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Default log context path used for logging.
    /// </summary>
    string DefaultLogContext { get; }

    /// <summary>
    /// Current thread status.
    /// </summary>
    ThreadStatus Status { get; set; }

    /// <summary>
    /// Most recently recorded thread status.
    /// </summary>
    ThreadStatus LastStatus { get; }

    /// <summary>
    /// Manually sets the thread name.  
    /// Can only be set once.
    /// </summary>
    /// <param name="name">Name to assign</param>
    void SetName(string name);

    /// <summary>
    /// Starts the thread.
    /// </summary>
    void Start();

    /// <summary>
    /// Requests the thread to stop.
    /// </summary>
    void Stop();

    /// <summary>
    /// Releases resources used by the thread.
    /// </summary>
    void Dispose();
}

/// <summary>
/// Extended interface for threads that support cancellation requests.
/// </summary>
public interface ICancellableThread : IThread
{
    /// <summary>
    /// Indicates whether cancellation has been requested via a token.
    /// </summary>
    bool IsCancellationRequested { get; }

    /// <summary>
    /// Requests cancellation of the thread from external code.
    /// </summary>
    void RequestCancel();
}

/// <summary>
/// Interface for thread managers responsible for thread registration and execution control.
/// </summary>
public interface IThreadManager
{
    /// <summary>
    /// Registers a thread instance.
    /// </summary>
    /// <param name="thread">Thread instance to register</param>
    void Register(IThread thread);

    /// <summary>
    /// Starts all registered threads.
    /// </summary>
    void StartAll();

    /// <summary>
    /// Requests all registered threads to stop.
    /// </summary>
    void StopAll();

    /// <summary>
    /// Starts the thread with the specified name.
    /// </summary>
    /// <param name="name">Name of the thread</param>
    /// <returns>True if started successfully, otherwise false</returns>
    bool Start(string name);

    /// <summary>
    /// Stops the thread with the specified name.
    /// </summary>
    /// <param name="name">Name of the thread</param>
    /// <returns>True if stopped successfully, otherwise false</returns>
    bool Stop(string name);

    /// <summary>
    /// Returns the number of threads that are currently running.
    /// </summary>
    /// <returns>Count of running threads</re
    int CountRunning();

    /// <summary>
    /// Returns the number of running threads at the specified priority level.
    /// </summary>
    /// <param name="level">Priority level</param>
    /// <returns>Count of running threads at the given level</returns>
    int CountRunning(ThreadPriorityLevel level);

    /// <summary>
    /// Returns all registered threads filtered by type.
    /// </summary>
    /// <typeparam name="T">Thread type</typeparam>
    /// <returns>Collection of threads of the specified type</returns>
    IEnumerable<T> GetThreads<T>() where T : class, IThread;

    /// <summary>
    /// Returns the thread with the specified name.
    /// </summary>
    /// <param name="name">Name of the thread</param>
    /// <returns>The found thread, or null if not found</returns>
    IThread? GetThread(string name);

    /// <summary>
    /// Automatically scans and registers all thread types defined in the specified assembly.
    /// </summary>
    /// <param name="assembly">Target assembly</param>
    void AutoRegisterAllThreads(Assembly assembly);

    /// <summary>
    /// Creates and registers a virtual thread using the specified name and action.
    /// </summary>
    /// <param name="name">Name of the virtual thread</param>
    /// <param name="action">Action to execute on each loop (int: current loop counter)</param>
    /// <param name="priority">Thread priority (default: High)</param>
    /// <param name="interval">Interval between loops (ms)</param>
    /// <param name="logPath">Log file path (null for default: Threading/{name}.txt)</param>
    void CreateVirtualThread(
        string name,
        Action<int> action,
        ThreadPriorityLevel priority = ThreadPriorityLevel.High,
        int interval = 0,
        string? logPath = null);
}

/// <summary>
/// Interface for thread registration.
/// </summary>
public interface IThreadRegistrar
{
    void Register(IThread thread);
}
