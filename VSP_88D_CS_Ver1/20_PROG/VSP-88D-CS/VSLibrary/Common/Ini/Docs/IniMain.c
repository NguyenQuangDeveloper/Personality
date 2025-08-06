/*!
 * \mainpage VSLibrary.Common.Ini
 *
 * \section intro Introduction
 *
 * This is a configuration management system for loading and accessing INI settings files using key-value access.
 * Both standalone and DI-based usage are supported, enabling structured management of various system configurations.
 * Internally, a Dictionary-based storage provides fast lookup and updates.
 * You can access configuration easily via the static VsIniManager class.
 *
 * \section namespace Namespace
 *
 * ```
 * VSLibrary.Common.Ini
 * ```
 *
 * \section classes Main Components
 *
 * - `IIniProvider`         : INI provider interface
 * - `IniBase`              : Base class for providers (shared Dictionary logic)
 * - `IIniManager`          : Configuration manager interface
 * - `VsIniManagerProxy`    : Actual INI file logic (Load/Save/Get/Set, etc.)
 * - `VsIniManager`         : Static entry point for configuration (external wrapper)
 * - `IniEnum`              : Helper file for defining section/key/file as Enums
 *
 * \section usage Basic Usage
 *
 * \code{.cs}
 * // 1. Initialize
 * VsIniManager.Initialize(@"D:\\Config\\app.ini");
 *
 * // 2. Get string value
 * string timeout = VsIniManager.Get("SYSTEM", "TIMEOUT");
 *
 * // 3. Get list value
 * var langs = VsIniManager.GetList("SYSTEM", "LANGUAGE");
 *
 * // 4. Set value
 * VsIniManager.Set("SYSTEM", "MODE", "AUTO");
 *
 * // 5. Save
 * VsIniManager.Save();
 * \endcode
 *
 * \section format INI Format Example
 *
 * ```ini
 * [SYSTEM]
 * TIMEOUT = 30
 * LANGUAGE = ko,en,vi
 * MODE = AUTO
 * ```
 *
 * \section di DI Extension
 *
 * When using DI, you can inject the `IIniManager` interface;
 * internally, `VsIniManagerProxy` acts as the actual implementation.
 * Example DI container registration:
 *
 * \code{.cs}
 * container.Register<IIniManager, VsIniManagerProxy>();
 * \endcode
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                                 |
 * |------------|---------|----------------|-------------------------------------------------------------|
 * | 2025-06-23 | 1.0.0   | Jang Minsu     | Initial implementation of INI parser and configuration manager module<br>Extensible structure for DI support    |
 * | 2025-07-02 | 1.0.0   | ChatGPT (GPT-4)| All core and public documentation fully translated to English<br> full Doxygen/markdown integration.           |
 *
 * \section license License
 *
 * This project is an internal library of VisionSemicon.
 * Redistribution and unauthorized copying are strictly prohibited.
 *
 * \section contact Contact
 *
 * Email: msjang@visionsemicon.co.kr
 */
