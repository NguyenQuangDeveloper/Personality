using System.Reflection;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Threading;

/// <summary>
/// Entry point for system-wide thread management.
/// This is a static class that holds an internal DI proxy for thread management.
/// </summary>
public static class ThreadManager
{
    /// <summary>
    /// The current thread manager implementation (injected via DI).
    /// </summary>
    private static IThreadManager? _manager;

    /// <summary>
    /// The dependency injection (DI) container instance.
    /// </summary>
    private static IContainer? _container;

    /// <summary>
    /// Initializes and registers the DI-based thread manager.
    /// </summary>
    /// <param name="container">DI container instance</param>
    public static void SetContainer(IContainer container)
    {
        _container = container;

        var manager = new ThreadManagerProxy();
        _manager = manager;

        _container.RegisterInstance<IThreadManager>(manager);
    }

    /// <summary>
    /// Returns the currently registered ThreadManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the ThreadManager has not been initialized.</exception>
    public static IThreadManager Instance => _manager ?? throw new InvalidOperationException("ThreadManager is not initialized.");

    /// <summary>
    /// Registers the specified thread.
    /// </summary>
    /// <param name="thread">Thread instance to register</param>
    public static void Register(IThread thread) => Instance.Register(thread);

    /// <summary>
    /// Starts all registered threads.
    /// </summary>
    public static void StartAll() => Instance.StartAll();

    /// <summary>
    /// Requests all registered threads to stop.
    /// </summary>
    public static void StopAll() => Instance.StopAll();

    /// <summary>
    /// Starts the thread with the specified name.
    /// </summary>
    /// <param name="name">Thread name</param>
    /// <returns>True if started successfully, otherwise false</returns>
    public static bool Start(string name) => Instance.Start(name);

    /// <summary>
    /// Requests the thread with the specified name to stop.
    /// </summary>
    /// <param name="name">Thread name</param>
    /// <returns>True if stopped successfully, otherwise false</returns>
    public static bool Stop(string name) => Instance.Stop(name);

    /// <summary>
    /// Automatically scans the specified assembly and registers all threads derived from VSThread.
    /// </summary>
    /// <param name="assembly">Target assembly</param>
    public static void AutoRegisterAllThreads(Assembly assembly) => Instance.AutoRegisterAllThreads(assembly);

    /// <summary>
    /// Returns the number of currently running threads.
    /// </summary>
    /// <returns>Number of running threads</returns>
    public static int CountRunning() => Instance.CountRunning();

    /// <summary>
    /// Returns the number of running threads at the specified priority level.
    /// </summary>
    /// <param name="level">Priority level</param>
    /// <returns>Number of running threads with the given priority</returns>
    public static int CountRunning(ThreadPriorityLevel level) => Instance.CountRunning(level);

    /// <summary>
    /// Returns all registered threads filtered by type.
    /// </summary>
    /// <typeparam name="T">Thread type</typeparam>
    /// <returns>Collection of threads of the specified type</returns>
    public static IEnumerable<T> GetThreads<T>() where T : class, IThread => Instance.GetThreads<T>();

   
    /// <summary>
    /// Finds and returns the thread with the specified name.
    /// </summary>
    /// <param name="name">Thread name</param>
    /// <returns>The found thread, or null if not found</returns>
    public static IThread? GetThread(string name) => Instance.GetThread(name);

    /// <summary>
    /// Creates and registers a virtual thread using the specified name and action.
    /// </summary>
    /// <param name="name">Name of the virtual thread</param>
    /// <param name="action">Action to execute on each loop (int: current loop counter)</param>
    /// <param name="priority">Thread priority (default: High)</param>
    /// <param name="interval">Interval between loops in ms (default: 0)</param>
    /// <param name="logPath">Log file path (null for default: Threading/{name}.txt)</param>
    public static void CreateVirtualThread(
        string name,
        Action<int> action,
        ThreadPriorityLevel priority = ThreadPriorityLevel.High,
        int interval = 0,
        string? logPath = null)
    {
        var thread = new VirtualThread(name, action, priority, interval, logPath);
        Register(thread);
    }
}

/// <summary>
/// Implementation class that performs actual thread management functions.
/// Implements the <see cref="IThreadManager"/> interface and controls all registered threads internally.
/// </summary>
public class ThreadManagerProxy : IThreadManager
{
    /// <summary>
    /// List for storing all registered threads.
    /// </summary>
    private readonly List<IThread> _threads = new();

    /// <summary>
    /// Registers a thread instance.
    /// Prevents duplicate registration of the same instance.
    /// </summary>
    /// <param name="thread">Thread instance to register</param>
    public void Register(IThread thread)
    {
        if (thread != null && !_threads.Contains(thread))
            _threads.Add(thread);
    }

    /// <summary>
    /// Starts all registered threads.
    /// </summary>
    public void StartAll()
    {
        foreach (var thread in _threads)
            thread.Start();
    }

    /// <summary>
    /// Requests all registered threads to stop.
    /// </summary>
    public void StopAll()
    {
        foreach (var thread in _threads)
            thread.Stop();
    }

    /// <summary>
    /// Sets the thread with the specified name to running state.
    /// </summary>
    /// <param name="name">Name of the thread to start</param>
    /// <returns>True if the thread was found and started; otherwise, false</returns>
    public bool Start(string name)
    {
        var thread = GetThread(name);
        if (thread != null)
        {
            thread.Status = ThreadStatus.Running;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the thread with the specified name to stopped state.
    /// </summary>
    /// <param name="name">Name of the thread to stop</param>
    /// <returns>True if the thread was found and stopped; otherwise, false</returns>
    public bool Stop(string name)
    {
        var thread = GetThread(name);
        if (thread != null)
        {
            thread.Status = ThreadStatus.Stopped;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the number of currently running threads.
    /// </summary>
    /// <returns>Number of threads in running state</returns>
    public int CountRunning() => _threads.Count(t => t.IsRunning);

    /// <summary>
    /// Returns the number of running threads at the specified priority level.
    /// </summary>
    /// <param name="level">Priority level</param>
    /// <returns>Number of running threads at the given priority</returns>
    public int CountRunning(ThreadPriorityLevel level) => _threads.Count(t => t.IsRunning && t.Priority == level);

    /// <summary>
    /// Returns all registered threads filtered by the specified type.
    /// </summary>
    /// <typeparam name="T">Type implementing <see cref="IThread"/></typeparam>
    /// <returns>Collection of threads of the specified type</returns>
    public IEnumerable<T> GetThreads<T>() where T : class, IThread => _threads.OfType<T>();

    /// <summary>
    /// Finds and returns the thread with the specified name.
    /// </summary>
    /// <param name="name">Thread name to search for</param>
    /// <returns>The found thread, or null if not found</returns>
    public IThread? GetThread(string name)
    {
        return _threads.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Automatically scans the given assembly for all types derived from <see cref="VSThread"/> and registers them.
    /// </summary>
    /// <param name="assembly">Target assembly</param>
    public void AutoRegisterAllThreads(Assembly assembly)
    {
        var threadBaseType = typeof(VSThread);

        var threadTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && threadBaseType.IsAssignableFrom(t) && !t.Name.Contains("Dynamic"))
            .ToList();

        foreach (var type in threadTypes)
        {
            if (Activator.CreateInstance(type) is VSThread instance)
            {
                Register(instance);
            }
        }
    }

    /// <summary>
    /// Creates and registers a virtual thread.
    /// </summary>
    /// <param name="name">Name of the virtual thread</param>
    /// <param name="action">Action to execute (receives loop counter as parameter)</param>
    /// <param name="priority">Thread priority</param>
    /// <param name="interval">Execution interval in ms</param>
    /// <param name="logPath">Log file path (if null, defaults to Threading/{name}.txt)</param>
    public void CreateVirtualThread(
        string name,
        Action<int> action,
        ThreadPriorityLevel priority = ThreadPriorityLevel.High,
        int interval = 0,
        string? logPath = null)
    {
        var thread = new VirtualThread(name, action, priority, interval, logPath);
        Register(thread);
    }
}

