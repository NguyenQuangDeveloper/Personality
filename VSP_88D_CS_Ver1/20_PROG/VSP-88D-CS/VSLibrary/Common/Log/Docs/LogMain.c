/*!
 * \mainpage VSLibrary.Common.Log
 *
 * \section intro Introduction
 *
 * This is the common logging system for VSLibrary.
 * Provides an extensible logging architecture based on `ILogProvider` and `ILogWriter`,
 * supporting both file-based logging and NLog output integration.
 *
 * \section namespace Namespace
 *
 * ```
 * VSLibrary.Common.Log
 * ```
 *
 * \section classes Main Components
 *
 * - `LogManager`     : Static entry point for the logging system (initialization/output)
 * - `ILogProvider`   : Interface for external log output (e.g., NLog)
 * - `ILogWriter`     : Interface for file-based log writers
 * - `BaseLogWriter`  : Implementation for file logging with date/size rotation
 * - `NLogProvider`   : NLog output implementation
 * - `LogOptions`     : Log settings DTO
 * - `LogType`        : Log type enumeration (Info, Warn, etc.)
 *
 * \section usage Basic Usage
 *
 * \code{.cs}
 * LogManager.Initialize("VsLog.txt", @"D:\Logs");
 * LogManager.Write("TEST");
 * LogManager.SetContext("Log/UI.txt");
 * LogManager.Write("Normal operation log");
 * \endcode
 *
 * \section output Example Log Output Path
 *
 * ```
 * D:\Logs\VsLog_2025-06-16[0000].txt
 * → Automatically split if over 5MB
 * → File name updated automatically when date changes
 *
 * D:\Logs\Log\UI_2025-06-16[0000].txt
 * → Automatically split if over 5MB
 * → File name updated automatically when date changes
 * ```
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                                 |
 * |------------|---------|----------------|-------------------------------------------------------------|
 * | 2025-06-16 | 1.0.0   | Jang Minsu     | Initial release: LogManager / NLogProvider / FileWriter    |
 * | 2025-06-23 | 1.0.0   | Jang Minsu     | Improved log context separation per thread using AsyncLocal<br>Automatic context detection for Write() / WriteDirect()<br>Added Doxygen comments to all major methods    |
 * | 2025-07-02 | 1.0.0   | ChatGPT (GPT-4)| All core and public documentation fully translated to English<br>full Doxygen/markdown integration.           |
 * 
 * \section license License
 *
 * This project is for internal corporate use only and is not permitted for external distribution.
 *
 * \section contact Contact
 *
 * Email: msjang@visionsemicon.co.kr
 */
