/*!
 * \mainpage VSLibrary.Common.MVVM
 *
 * \section intro Introduction
 *
 * The VSLibrary MVVM system supports automatic ViewModel binding, region-based navigation, and dependency injection management across multiple platforms (WPF, WinForms, Blazor, MAUI, etc.).
 * All modules are included under the `VSLibrary.Common.MVVM` namespace.
 *
 * \section modules Main Components
 *
 * - **Interfaces** : Core interfaces for DI, region navigation, and lifecycle management
 * - **Core**       : Core MVVM containers and command logic (RegionManager, VSContainer, etc.)
 * - **Locators**   : Automatic ViewModel locator and binder logic
 * - **ViewModels** : ViewModelBase and Blazor-specific extensions
 * - **Behaviors**  : View-related behaviors such as RegionBehavior and WindowDragBehavior
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                                 |
 * |------------|---------|----------------|-------------------------------------------------------------|
 * | 2025-06-18 | 1.0.0   | Jang Minsu     | Initial MVVM release (Region, ViewModelLocator included)    |
 * | 2025-06-26 | 1.0.0   | Jang Minsu     | Added WindowDragBehavior.cs (make WPF windows draggable)    |
 * | 2025-06-27 | 1.0.0   | Jang Minsu     | Added ViewManager, IViewManager, expanded RegionManager, INavigator. Enhanced ViewModel→View navigation and doc update. |
 * | 2025-07-02 | 1.0.0   | ChatGPT (GPT-4)| All core and public documentation fully translated to English<br> full Doxygen/markdown integration.           |
 *
 * \section license License
 *
 * This project is an internal module of VisionSemicon.
 * Any external distribution or unauthorized use is strictly prohibited.
 *
 * \section contact Contact
 *
 * 📧 Email: msjang@visionsemicon.co.kr
 */
