namespace VSLibrary.Common.App;

/// <summary>
/// Provides a mechanism to prevent multiple instances of the application using a system-wide mutex.
/// </summary>
public sealed class SingleInstanceGuard : IDisposable
{
    private Mutex? _mutex;
    private readonly string _mutexName;
    private bool _isOwner;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleInstanceGuard"/> class.
    /// </summary>
    /// <param name="appId">A unique identifier for the application instance (used as the mutex name).</param>
    public SingleInstanceGuard(string appId)
    {
        _mutexName = $"Global\\{appId}";
    }

    /// <summary>
    /// Attempts to acquire the mutex and determine if this is the first instance.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is the first instance of the application;
    /// <c>false</c> if another instance is already running.
    /// </returns>
    public bool TryAcquire()
    {
        _mutex = new Mutex(true, _mutexName, out _isOwner);
        return _isOwner;
    }

    /// <summary>
    /// Releases the mutex if owned and disposes the associated resources.
    /// </summary>
    public void Dispose()
    {
        if (_isOwner)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }
}
