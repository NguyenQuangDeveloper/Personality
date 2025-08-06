/*!
 * \mainpage VSLibrary.Common.App
 *
 * \section intro Introduction
 *
 * The App module provides the WPF application bootstrap entry point.
 * It handles DI container registration, thread initialization, localization,
 * and single-instance mutex handling using `SingleInstanceGuard`.
 *
 * \section namespace Namespace
 *
 * ```
 * VSLibrary.Common.App
 * ```
 *
 * \section classes Main Components
 *
 * - `App.xaml / App.xaml.cs` : WPF entry point with OnStartup, OnExit
 * - `SingleInstanceGuard`    : Prevents multiple instances (mutex-based)
 * - `AppInitializer`         : (optional) Type registration helper
 *
 * \section usage Basic Usage
 *
 * \code{.cs}
 * _guard = new SingleInstanceGuard("AppName");
 * if (!_guard.TryAcquire())
 * {
 *     Shutdown();
 *     return;
 * }
 *
 * RegisterTypes();
 * mainWindow.Show();
 * \endcode
 *
 * \section flow Startup Flow
 *
 * 1. Acquire mutex
 * 2. Register DI/log/thread/localization
 * 3. Show main window
 * 4. Clean up on exit
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                 |
 * |------------|---------|----------------|---------------------------------------------|
 * | 2025-07-22 | 1.0.0   | Jang Minsu     | Initial App module doc                      |
 * | 2025-07-22 | ChatGPT (GPT-4o) | Doxygen structure & documentation written |
 *
 * \section license License
 *
 * Internal module of VisionSemicon/Dreamine.
 * Unauthorized redistribution is strictly prohibited.
 *
 * \section contact Contact
 *
 * Email: msjang@visionsemicon.co.kr
 */
