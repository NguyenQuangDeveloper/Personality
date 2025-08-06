# 📘 Log Module

This is the common logging system for VSLibrary.   
It provides an **extensible logging architecture** based on `ILogProvider` and `ILogWriter`,  
integrating both file-based logging and NLog output.

---

## 📦 Namespace

```
VSLibrary.Common.Log
```

---

## 🧱 Main Classes

| Name            | Role                                                              |
| --------------- | ----------------------------------------------------------------- |
| `LogManager`    | Static entry point for the logging system (initialization/output) |
| `ILogProvider`  | Interface for external log output (e.g., NLog)                    |
| `ILogWriter`    | Interface for file-based log writers                              |
| `BaseLogWriter` | File-based implementation (rotation by date/size)                 |
| `NLogProvider`  | NLog output implementation                                        |
| `LogOptions`    | Log settings DTO                                                  |
| `LogType`       | Enum for log levels (Info, Warn, etc.)                            |


---

## 🧩 Class Diagram

> See the `Log.cd` file
(Visualize in DGML or Visual Studio Class Diagram tools)

```
Common.Log
├─ LogManager
│   ├─ ILogProvider → NLogProvider
│   └─ ILogWriter   → BaseLogWriter
└─ LogOptions
```

---

## ⚙️  Basic Usage

### 1. nitialization (once at app startup)

```csharp
LogManager.Initialize(); 
// or
LogManager.Initialize("System/Boot.txt", @"D:\Logs");
```

### 2. Specify Log Context

```csharp
LogManager.SetContext("Log/UI.txt");
```

### 3. Write Log

```csharp
LogManager.Write("Log button clicked in MainWindow screen");
```

---

## 📤 Log Level Examples

```csharp
LogManager.Info("Normal operation log");
LogManager.Warn("Warning occurred");
LogManager.Debug("Debug information");
LogManager.Error("Error occurred");
LogManager.Error("Exception occurred", ex);
```
 > These logs are created under the executable path and are intended for debugging only.
---

## 📂 Log Storage Example

```
D:\Logs\Log\UI_2025-06-16[0000].txt
→ Automatically split if over 5MB
→ File name reflects date change automatically
```

---

## 🔧 Log Format Example

```csharp
new LogOptions
{
    LogDirectory = @"D:\Logs",
    MaxFileSizeMB = 5,
    EnableAutoZip = false,
    LogFormat = "[{type}] {time} > {message}"
};
```


## Using with DI
> You can inject ILogManager into a ViewModel or service using a DI container.

- Register with DI container
```csharp
In App.xaml.cs, called once at startup:
    LogManager.SetContainer(_vsContainer);
```
---

- Inject and use in a ViewModel
```csharp
private readonly ILogManager _logManager;

public MainWindowViewModel(ILogManager logManager)
{
    _logManager = logManager;
    _logManager.SetContext("UI/UI.txt");
    _logManager.Write("MainWindow ViewModel created");
}
```

---

## ⚠️ Usage Note
> If you use `LogManager.SetContext(...)` from multiple classes (Thread, ViewModel, etc.)  
> at the same time, log messages may be mixed between files due to internal ThreadStatic caching.
```csharp
// If other locations do not specify a folder path, issues may occur.
LogManager.SetContext("Thread/Thread.txt"); 
LogManager.Write($"RunProc CommThread: {++_count}");
```
---

## 📌 Quick Reference

| Feature            | Description                 |
| ------------------ | --------------------------- |
| `Initialize()`     | One-line setup              |
| `SetContext()`     | Change file location        |
| `Write()`          | Write message               |
| `Info()`, `Warn()` | External log output (NLog)  |
| `BaseLogWriter`    | Date + index-based rotation |
| `Log.cd`           | Visual structure diagram    |


---

## 📅 Update History

| Date       | Author         | Description / Notes                     |
|------------|---------------|-----------------------------------------|
| 2025-06-16 | Jang Minsu     | Initial creation in Korean              |
| 2025-07-02 | ChatGPT (GPT-4)| Full comment/documentation translated to English |

---

📅 Document Date: 2025-06-16  
🖋️ Author: Minsu Jang