# 📘 INI Configuration Module

This is the INI configuration system included in the `VSLibrary.Common.Ini` namespace.  
Supports both standalone and DI-based usage. Loads `.ini` files and provides key-value access to configuration data.

---

## 📦 Namespace

```
VSLibrary.Common.Ini
```

---

## 🧱 Main Classes

| Class Name          | Description                                           |
| ------------------- | ----------------------------------------------------- |
| `IniBase`           | Base provider class. Manages Dictionary-based storage |
| `VsIniManager`      | Static entry point. Provides public API access        |
| `VsIniManagerProxy` | Implements actual data logic (Load/Save/Set/Get)      |
| `IIniManager`       | Configuration interface (used for DI or proxy)        |
| `IIniProvider`      | Configuration provider interface                      |
| `IniEnum.cs`        | Helper for section/key enums                          |


---

## 🧩 Class Structure

```
Ini System
├─ IIniProvider
│    └─ IniBase (abstract)
│
├─ IIniManager
│    └─ VsIniManagerProxy
│
└─ VsIniManager (static wrapper)
```

---

## ⚙️ Basic Usage

### 1. Initialization

```csharp
VsIniManager.Initialize(@"D:\Config\app.ini");
```

### 2. Get Value

```csharp
var timeout = VsIniManager.Get("SYSTEM", "TIMEOUT");
```

### 3. Get List

```csharp
var langs = VsIniManager.GetList("SYSTEM", "LANGUAGE");
```

### 4. Set Value
```csharp
VsIniManager.Set("SYSTEM", "RESET_DOOR_ALARM_SKIP_TIME", "0");
```

### 5. Save Values

```csharp
VsIniManager.Save();
```

---

## 📝 INI Example

```ini
[SYSTEM]
TIMEOUT = 30
LANGUAGE = ko,en,vi
```

---

## 🔧 Extension with DI

- When using DI, call `SetContainer(IContainer)`
- to register `IIniManager` in the DI container and use it via dependency injection.

---

## 📌 Feature Summary

| Method                     | Description                          |
| -------------------------- | ------------------------------------ |
| `Initialize(path)`         | Load INI file                        |
| `Get(section, key)`        | Return value as string               |
| `GetList(section, key)`    | Return value as comma-separated list |
| `Set(section, key, value)` | Set configuration value              |
| `Save(path)`               | Save current configuration           |


## 📅 Update History

| Date       | Author         | Description / Notes                     |
|------------|---------------|-----------------------------------------|
| 2025-06-23 | Jang Minsu     | Initial creation in Korean              |
| 2025-07-02 | ChatGPT (GPT-4)| Full comment/documentation translated to English |

---

📅 Document Date: 2025-06-23   
🖋️ Author: Minsu Jang


