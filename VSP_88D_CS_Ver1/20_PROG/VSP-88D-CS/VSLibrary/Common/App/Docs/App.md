# 📘 App Entry Module

This is the application entry system defined under the `VSLibrary.Common.App` namespace.  
It initializes DI, threading, localization, and prevents multiple application instances via `SingleInstanceGuard`.

---

## 📦 Namespace

```
VSLibrary.Common.App
```

---

## 🧱 Main Classes

| Class Name             | Description                                              |
|------------------------|----------------------------------------------------------|
| `App`                  | WPF application entry point (`App.xaml.cs`)              |
| `App.xaml`             | Defines application-level resources and styles           |
| `SingleInstanceGuard`  | Prevents multiple application instances using mutex      |
| `AppInitializer` *(opt)| (Planned) Handles DI and registration for App startup    |

---

## 🧩 Class Structure

```
App Entry System
├─ App.xaml / App.xaml.cs
│ ├─ OnStartup()
│ ├─ OnExit()
│ └─ RegisterTypes()
│
└─ SingleInstanceGuard
├─ TryAcquire()
└─ Dispose()
```

---

## ⚙️ Basic Usage

### 1. Ensure Single Instance

```csharp
_singleInstanceGuard = new SingleInstanceGuard("MyApp");

if (!_singleInstanceGuard.TryAcquire())
{
    MessageBox.Show("Application is already running.");
    Shutdown();
    return;
}
```

### 2. Register Components

```csharp
_vsContainer.AutoInitialize(Assembly.GetExecutingAssembly());
ThreadManager.SetContainer(_vsContainer);
ThreadManager.StartAll();
VsLocalizationManager.Load(LanguageType.Korean);
```

### 3. Show Main Window

```csharp
if (_vsContainer.Resolve(typeof(MainWindow)) is MainWindow mainWindow)
{
    mainWindow.Show();
}
```
---

## 🛠 Features

| Feature                | Description                                                     |
| ---------------------- | --------------------------------------------------------------- |
| DI container setup     | Registers `VSContainer` and all required components             |
| Thread manager linking | Registers and starts threads via `ThreadManager`                |
| Logging                | Initializes logging system via `LogManager`                     |
| Localization           | Loads language dictionary using `VsLocalizationManager`         |
| Single instance mutex  | Prevents duplicate execution via `SingleInstanceGuard`          |
| Resource proxy         | Supports dynamic language binding via `StaticLocalizationProxy` |

---

## 📝 Example Resource (App.xaml)

```Xaml
<Application.Resources>
    <local:StaticLocalizationProxy x:Key="LangProxy" />
</Application.Resources>
```
---

## 🧩 Doxygen Target Class

- App (partial class): OnStartup, OnExit, RegisterTypes
- SingleInstanceGuard: TryAcquire, Dispose             | Save current configuration           |

---

## 🔧 Extension Options
 - Add AppInitializer.cs to split RegisterTypes()
 - Add AppOptions.cs to parse external config / command-line
 - Support App.md, App.cd, .dgml, .mm for internal doc tracking

 ---

 ## Feature Summary
| Method            | Description                               |
| ----------------- | ----------------------------------------- |
| `OnStartup()`     | Called when WPF app starts                |
| `RegisterTypes()` | Registers types, DI, logging, and threads |
| `TryAcquire()`    | Verifies if app instance is the only one  |
| `Dispose()`       | Releases system mutex                     |
| `ShowWindow()`    | Displays the main WPF window              |

 ---

## 📅 Update History

| Date       | Author           | Description / Notes                              |
| ---------- | ---------------- | ------------------------------------------------ |
| 2025-07-22 | Jang Minsu       | Document structure and layout defined            |
| 2025-07-22 | ChatGPT (GPT-4o) | Initial content and structure written in English |

---

📅 Document Date: 2025-07-22   
🖋️ Author: Minsu Jang


