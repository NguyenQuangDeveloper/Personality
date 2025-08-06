namespace VSLibrary.Threading;

/// <summary>
/// Represents the execution priority level of a system thread.
/// Used for core affinity and CPU resource prioritization.
/// </summary>
public enum ThreadPriorityLevel
{
    /// <summary> 
    /// High priority: For real-time tasks such as motion boards and IO control. 
    /// </summary>
    High,

    /// <summary> 
    /// Medium priority: For main logic such as FSM and equipment sequences. 
    /// </summary>
    Medium,

    /// <summary> 
    /// Low priority: For non-real-time tasks such as communication and monitoring. 
    /// </summary>
    Low
}

/// <summary>
/// Represents the current execution status of a system thread.
/// Used as a standard for FA equipment state monitoring and logging.
/// </summary>
public enum ThreadStatus
{
    /// <summary> 
    /// Uninitialized or unknown state. 
    /// </summary>
    Unknown,

    /// <summary> 
    /// Idle (not performing any work). 
    /// </summary>
    Idle,

    /// <summary> 
    /// Running normally. 
    /// </summary>
    Running,

    /// <summary> 
    /// Paused state. 
    /// </summary>
    Paused,

    /// <summary> 
    /// Explicitly stopped by the user. 
    /// </summary>
    Stopped,

    /// <summary> 
    /// Work completed normally (e.g., for one-shot threads). 
    /// </summary>
    Completed,

    /// <summary> 
    /// Error or fault state. 
    /// </summary>
    Error
}