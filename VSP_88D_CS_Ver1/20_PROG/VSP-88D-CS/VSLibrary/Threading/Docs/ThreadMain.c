/*!
 * \mainpage VSLibrary.Threading
 *
 * \section intro Introduction
 *
 * The VSLibrary high-performance thread management module.
 * Based on the `IThread` interface, it supports automatic registration of various thread types,
 * integrated management with priority-based scheduling, and robust exception handling.
 *
 * \section namespace Namespace
 *
 * ```
 * VSLibrary.Threading
 * ```
 *
 * \section classes Main Components
 *
 * - `ThreadBase<TSelf>` : Generic base class for user-defined threads
 * - `VSThread`          : Concrete base class for custom thread implementations
 * - `ThreadManager`     : Thread manager for registration, execution, stop, and lookup
 * - `ThreadFactory`     : Utility for automatic thread instantiation
 * - `CpuAffinityHelper` : Pin threads to specific CPU cores
 * - `TimeResolutionHelper` : Set high-precision (1ms) system timer
 *
 * \section usage Usage Example
 *
 * \code{.cs}
 * public class CommThread : VSThread
 * {
 *     private long _count = 0;
 *     public CommThread()
 *     {
 *         Priority = ThreadPriorityLevel.High;
 *         Status = ThreadStatus.Running;
 *     }
 *     protected override void RunProc()
 *     {
 *         LogManager.Write($"RunProc CommThread: {++_count}");
 *     }
 *     protected override void OnException(Exception ex)
 *     {
 *         LogManager.Error("Exception in CommThread", ex);
 *     }
 * }
 *
 * // Auto-register and start all threads
 * ThreadManager.AutoRegisterAllThreads(Assembly.GetExecutingAssembly());
 * ThreadManager.Instance.StartAll();
 * \endcode
 *
 * \section priority Thread Priority Levels
 *
 * | Level   | Description                          |
 * |---------|--------------------------------------|
 * | High    | Real-time tasks (motion, IO, etc.)   |
 * | Medium  | FSM and sequential logic             |
 * | Low     | Communication, logging, etc.         |
 *
 * \section interval Monitoring Results (Example)
 *
 * - High-precision cycle timing (average 8–14ms)
 * - Most High Priority threads run at ~8.8ms interval
 * - Low Priority threads: 12–19ms, more variable under load
 * - Less than 0.1% of threads delayed over 100ms
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                                 |
 * |------------|---------|----------------|-------------------------------------------------------------|
 * | 2025-06-18 | 1.0.0   | Jang Minsu     | Initial release: VSThread / ThreadManager / CpuAffinityHelper    |
 * | 2025-06-19 | 1.0.0   | Jang Minsu     | Added full Doxygen comments for all ThreadBase<T> methods<br> Documented internal flow for SetName, ThreadLoop, RunProc, GetSleepDuration, etc.<br>Switched from inheritdoc to explicit method comments for public APIs<br>Documentation rule started: "All public APIs must have direct comments" |
 * | 2025-06-20 | 1.0.0   | Jang Minsu     | Added logPath parameter to VirtualThread for log file customization<br>Extended ThreadFactory.CreateVirtual() API to support logPath<br>Improved LogManager.SetContext() timing (set in constructor) <br>Fixed log interference when running multiple thread files in parallel <br>Validated multi-threaded logging with 100+ repeated tests |
 * | 2025-06-26 | 1.0.0   | Jang Minsu     | Removed direct dependency from ThreadFactory to ThreadManager (eliminated circular dependency)<br>Introduced IThreadRegistrar interface for clearer responsibility separation<br>Clarified responsibility for internal registration in ThreadManager.CreateVirtualThread<br>Final design review: confirmed stable, cycle-free architecture   |
 * | 2025-07-02 | 1.0.0   | ChatGPT (GPT-4)| All core and public documentation fully translated to English<br> full Doxygen/markdown integration.           |
 *
 * \section license License
 *
 * This project is strictly for internal company use only.
 * Redistribution or external release is strictly prohibited.
 *
 * \section contact Contact
 *
 * Email: msjang@visionsemicon.co.kr
 */
