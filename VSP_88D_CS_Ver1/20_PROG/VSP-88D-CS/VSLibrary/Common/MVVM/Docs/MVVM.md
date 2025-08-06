# 🧭 MVVM Core Structure

This is the architecture overview for the VSLibrary MVVM system.  
It supports automatic ViewModel binding, Region management, and DI across multiple platforms (WPF, WinForms, Blazor, and more).

---

## 📦 Namespace Structure

```plaintext
VSLibrary.Common.MVVM
├── Interfaces  # Core interfaces (DI, Region, lifecycle, etc.)
├── Core        # MVVM containers and command infrastructure
├── Locators    # ViewModel auto-wiring and mapping
├── ViewModels  # ViewModelBase hierarchy
└── Behaviors   # Region and UI behaviors   
```

---

## 🧩 Class Diagram Overview (See MVVM.cd)
```plaintext
Common.MVVM (structure)
├── Interfaces
│   ├── IContainer                  → DI container abstraction
│   ├── IActivatable                → ViewModel activation/deactivation lifecycle
│   ├── INavigator                  → Navigation abstraction for ViewModel-driven navigation
│   ├── IRegionAware                → Region navigation context receiver
│   ├── IRegionManager              → Region registration/navigation manager
│   └── IViewManager                → ViewModel ↔ View navigation/creation abstraction
├── Core
│   ├── RegionManager               → Implements IRegionManager
│   ├── VSContainer                 → Implements IContainer (DI register/resolve)
│   ├── ViewManager                 → Implements IViewManager (window/region navigation)
│   └── RelayCommand                → ICommand implementation (MVVM binding)
├── Locators
│   ├── ViewModelLocatorConverter   → For XAML auto-wiring
│   └── ViewModelLocator            → ViewModel mapping and resolver
├── ViewModels
│   ├── BlazorViewModelBase         → Blazor-specific ViewModel base
│   └── ViewModelBase               → IActivatable, general WPF base ViewModel
├── Behaviors
│   ├── WindowDragBehavior          → Window dragging by mouse
│   └── RegionBehavior              → Automatic region registration on View load
```

---

## 🧠 Core Components Summary

| Component                   | Description                                               |
| --------------------------- | --------------------------------------------------------- |
| `VSContainer`               | Dependency injection container, service/viewmodel creator |
| `RegionManager`             | Manages region-based navigation (WPF/Blazor)              |
| `ViewManager`               | Handles navigation and window/region display              |
| `RelayCommand`              | MVVM command pattern implementation                       |
| `ViewModelLocator`          | Auto-wires View ↔ ViewModel (in XAML/code-behind)         |
| `ViewModelLocatorConverter` | XAML ViewModel auto-binding converter                     |
| `IActivatable`              | ViewModel lifecycle (activation/deactivation)             |
| `IRegionAware`              | Receives context/state on region navigation               |
| `INavigator`                | Interface for direct navigation from ViewModel            |
| `WindowDragBehavior`        | UI behavior for draggable Windows                         |
| `RegionBehavior`            | Binds ViewModel to RegionManager on View load             |

---

## ⚙️ Usage Examples

### 🪟 1. DI Registration and Region Usage

```csharp
// App.xaml.cs
protected VSContainer _vsContainer = VSContainer.Instance;

protected void RegisterTypes()
{
    _vsContainer.RegisterInstance<IContainer>(_vsContainer);
    _vsContainer.AutoInitialize(Assembly.GetExecutingAssembly()); 
}

protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    RegisterTypes();

    var viewManager = _vsContainer.Resolve<IViewManager>();

    viewManager.Show<OperatorViewModel>("MainMenu"); 
    viewManager.Show<ManagerViewModel>();          
}
```

---
### 🪟 2. RegionBehavior Usage

```xml
xmlns:vsmvvm="clr-namespace:VSLibrary.Common.MVVM.Behaviors;assembly=VSLibrary"

<ContentControl Grid.Row="1" vsmvvm:RegionBehavior.RegionName="MainMenu" />
```
---

### 🪟 3. WindowDragBehavior Usage
For borderless or transparent windows, simply add WindowDragBehavior to enable dragging:

```xml
<Window
    WindowStyle="None"
    AllowsTransparency="True"
    Background="Transparent"
    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    xmlns:mvvm="clr-namespace:Common.MVVM.Behaviors">

    <i:Interaction.Behaviors>
        <mvvm:WindowDragBehavior />
    </i:Interaction.Behaviors>

    <!-- UI content here -->
</Window>
```
> TIP: To make a specific UI element draggable, attach the behavior to a Grid/Border as needed.

--- 

### 🪟 4. ViewModel Auto-Wiring

- If View and ViewModel share naming conventions (e.g., MainWindow.xaml/MainWindowViewModel.cs),
- ViewModelLocator auto-wiring will work out-of-the-box.

---

## 🔄 MVVM ViewModel Binding Patterns (A–D)

### ✅ Pattern A: VSContainer with ViewModelLocator

```csharp
// App.xaml.cs
protected VSContainer _vsContainer { get; private set; } = VSContainer.Instance;

protected void RegisterTypes()
{
    _vsContainer.RegisterInstance<IContainer>(_vsContainer);
    _vsContainer.RegisterInstance<IRegionManager>(_vsContainer.RegionManager);
    _vsContainer.Register<MainWindowViewModel>();

    ViewModelLocator.Initialize(_vsContainer);
}

protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    RegisterTypes();
    ShowWindow();
}

private void ShowWindow()
{
    if (_vsContainer.Resolve(typeof(MainWindow)) is MainWindow mainWindow)
    {
        mainWindow.Show();
    }
}
```

```xml
<!-- App.xaml -->
<!-- StartupUri Delete -->
<Application ... StartupUri="">
```
---

### ✅ Pattern B: XAML Direct ViewModel Registration

```xml
<!-- MainWindow.xaml -->
<Window xmlns:vm="clr-namespace:MvvmSample.GUI">
    <Window.DataContext>
        <vm:MainWindowViewModel />
    </Window.DataContext>
</Window>
```

> ⚠️ This approach may conflict with DI containers, ViewModelLocator, or automatic ViewModel binding (AutoWireViewModel).

---

### ✅ Pattern C: Direct Code-Behind

```csharp
// MainWindow.xaml.cs
using MvvmSample.GUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }
}
```

> ⚠️ This pattern is outside standard MVVM, and may be deprecated or problematic if you want to use DI or automatic ViewModel-View binding in the future.

---

### ✅ Pattern D: ViewModelLocator + Mapping Registration

```csharp
// App.xaml.cs
protected void RegisterTypes()
{
    _vsContainer.RegisterInstance<IContainer>(_vsContainer);
    _vsContainer.RegisterInstance<IRegionManager>(_vsContainer.RegionManager);
    _vsContainer.RegisterView(typeof(MainWindow), typeof(MainWindowViewModel));
    ViewModelLocator.Initialize(_vsContainer);
}
```

```xml
<!-- MainWindow.xaml -->
<Window
    xmlns:mvvm="clr-namespace:VSLibrary.Common.MVVM.Locators;assembly=VSLibrary"
    mvvm:ViewModelLocator.AutoWireViewModel="True">
</Window>
```

---

## 📚 Related Files

| File Name               | Description                                   |
| ----------------------- | --------------------------------------------- |
| `MVVM.cd`               | Class diagram                                 |
| `VSContainer.cs`        | DI container (core of dependency injection)   |
| `RegionManager.cs`      | Region navigation and management logic        |
| `ViewModelBase.cs`      | Implements IActivatable, ViewModel base class |
| `RegionBehavior.cs`     | Behavior for binding Views to RegionManager   |
| `WindowDragBehavior.cs` | Behavior for making windows draggable         |

---
## 📅 Update History

| Date       | Author         | Description / Notes                     |
|------------|---------------|-----------------------------------------|
| 2025-06-16 | Jang Minsu     | Initial creation in Korean              |
| 2025-07-02 | ChatGPT (GPT-4)| Full comment/documentation translated to English |

---

📅 Document Date: 2025-06-16  
🖋️ Author: Minsu Jang
