# 📘 Thread Module

The VSLibrary Thread module provides a unified thread management system for all applications.  
It supports structural expansion based on the `IThread` interface and offers  
**automatic registration, scheduled execution, priority control, and exception management.**

---

## 📦 Namespace

```
VSLibrary.Threading
```

---

## 🧱 Main Classes

| Name                   | Role/Description                                   |
| ---------------------- | -------------------------------------------------- |
| `ThreadBase<TSelf>`    | Generic base class for thread implementation       |
| `VSThread`             | Concrete base class for user-defined threads       |
| `ThreadManager`        | Manager for thread registration and global control |
| `ThreadFactory`        | Generic thread instantiator/factory                |
| `CpuAffinityHelper`    | Utility for pinning threads to specific CPU cores  |
| `TimeResolutionHelper` | Utility for high-precision (1ms) timer control     |


---

## 🧩 Class Diagram

```css
Thread
├─ ThreadBase<TSelf> : IThread
│  └─ VSThread
│
├─ ThreadManager
│  ├─ Register / StartAll / StopAll / AutoRegister
│  └─ GetThreads<T> / CountRunning
│
├─ ThreadFactory → Create<T>()
├─ CpuAffinityHelper → SetAffinity(coreIndex)
└─ TimeResolutionHelper → Enable1msResolution(), Disable1msResolution()

```

---

## ⚙️ Quick Usage

### 1. Implementing a Custom Thread

```csharp
public class CommThread : VSThread
{
    private long _count = 0;

    public CommThread()
    {         
        Priority = ThreadPriorityLevel.High;
        Status = ThreadStatus.Running; 
        SetDefaultLogContext("Thread/Thread.txt");
    }

    protected override void RunProc()
    {
        LogManager.Write($"RunProc CommThread: {++_count}");
    }

    protected override void OnException(Exception ex)
    {
        LogManager.Error("Exception in CommThread", ex);
    }
}
```

---

### 2. Auto Registration & Execution

```csharp
ThreadManager.AutoRegisterAllThreads(Assembly.GetExecutingAssembly());
ThreadManager.Instance.StartAll();
```

---

## 📌 ThreadPriorityLevel

| Level    | Description                                         |
| -------- | --------------------------------------------------- |
| `High`   | For hard real-time tasks (motion board, IO, etc.)   |
| `Medium` | For standard process control (FSM, equipment logic) |
| `Low`    | For non-critical tasks (communication, logging)     |


---

## 🔧 Advanced Features

| Feature                                      | Description                                                |
| -------------------------------------------- | ---------------------------------------------------------- |
| `CpuAffinityHelper.SetAffinity(index)`       | Pin a thread to a specific CPU core                        |
| `TimeResolutionHelper.Enable1msResolution()` | Set system timer to 1ms resolution (high precision timing) |
| `ThreadBase.RunProc()`                       | Main thread execution logic override point                 |
| `ThreadFactory.Create<T>()`                  | Instantiates and registers a thread automatically          |
| `ICancellableThread`                         | Thread with cancellation token support                     |


---
## Average & Maximum Thread Intervals (20 logical cores @ 2.4GHz) 

| Thread Name                  | Count | Avg Interval (ms) | Max Interval (ms) | Min Interval (ms) | Over 100ms Count |
| ---------------------------- | ----- | ----------------: | ----------------: | ----------------: | ---------------: |
| CommThread                   | 1709  |              8.84 |                37 |                 1 |                0 |
| Overload\_High\_Thread\_00   | 1722  |              8.77 |                47 |                 1 |                0 |
| Overload\_High\_Thread\_01   | 1705  |              8.86 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_02   | 1700  |              8.89 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_03   | 1697  |              8.89 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_04   | 1699  |              8.88 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_05   | 1696  |              8.89 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_06   | 1693  |              8.90 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_07   | 1689  |              8.92 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_08   | 1685  |              8.94 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_09   | 1686  |              8.93 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_10   | 1681  |              8.94 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_11   | 1682  |              8.94 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_12   | 1683  |              8.94 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_13   | 1672  |              8.99 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_14   | 1672  |              8.99 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_15   | 1674  |              8.98 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_16   | 1678  |              8.96 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_17   | 1671  |              8.99 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_18   | 1670  |              8.99 |                69 |                 1 |                0 |
| Overload\_High\_Thread\_19   | 1664  |              9.02 |                69 |                 1 |                0 |
| Overload\_Low\_Thread\_00    | 1484  |             11.18 |               170 |                 2 |                2 |
| Overload\_Low\_Thread\_01    | 1441  |             11.51 |               170 |                 2 |                2 |
| Overload\_Low\_Thread\_02    | 1380  |             12.02 |               171 |                 2 |                2 |
| Overload\_Low\_Thread\_03    | 1344  |             12.34 |               172 |                 2 |                2 |
| Overload\_Low\_Thread\_04    | 1314  |             12.61 |               172 |                 2 |                2 |
| Overload\_Low\_Thread\_05    | 1288  |             12.87 |               173 |                 2 |                2 |
| Overload\_Low\_Thread\_06    | 1260  |             13.15 |               173 |                 2 |                2 |
| Overload\_Low\_Thread\_07    | 1233  |             13.44 |               174 |                 3 |                2 |
| Overload\_Low\_Thread\_08    | 1210  |             13.68 |               175 |                 3 |                2 |
| Overload\_Low\_Thread\_09    | 1192  |             13.87 |               176 |                 3 |                2 |
| Overload\_Low\_Thread\_10    | 1172  |             14.10 |               176 |                 3 |                2 |
| Overload\_Low\_Thread\_11    | 1156  |             14.29 |               177 |                 3 |                2 |
| Overload\_Low\_Thread\_12    | 1134  |             14.53 |               177 |                 4 |                2 |
| Overload\_Low\_Thread\_13    | 1114  |             19.00 |               181 |                10 |                2 |
| Overload\_Low\_Thread\_14    | 1100  |             19.39 |               182 |                11 |                2 |
| Overload\_Low\_Thread\_15    | 1095  |             19.50 |               183 |                11 |                2 |
| Overload\_Low\_Thread\_16    | 1089  |             19.61 |               183 |                11 |                2 |
| Overload\_Low\_Thread\_17    | 1082  |             19.72 |               184 |                12 |                2 |
| Overload\_Low\_Thread\_18    | 1082  |             19.72 |               185 |                12 |                2 |
| Overload\_Low\_Thread\_19    | 1079  |             19.78 |               212 |                12 |                3 |
| Overload\_Medium\_Thread\_00 | 1184  |             13.34 |               149 |                 2 |                1 |
| Overload\_Medium\_Thread\_01 | 1171  |             13.47 |               150 |                 2 |                1 |
| Overload\_Medium\_Thread\_02 | 1160  |             13.60 |               150 |                 3 |                1 |
| Overload\_Medium\_Thread\_03 | 1147  |             13.76 |               151 |                 3 |                1 |
| Overload\_Medium\_Thread\_04 | 1138  |             13.87 |               151 |                 3 |                1 |
| Overload\_Medium\_Thread\_05 | 1129  |             13.98 |               152 |                 4 |                1 |
| Overload\_Medium\_Thread\_06 | 1120  |             14.10 |               152 |                 4 |                1 |
| Overload\_Medium\_Thread\_07 | 1112  |             14.21 |               153 |                 4 |                1 |
| Overload\_Medium\_Thread\_08 | 1108  |             14.27 |               153 |                 4 |                1 |
| Overload\_Medium\_Thread\_09 | 1102  |             14.36 |               154 |                 4 |                1 |
| Overload\_Medium\_Thread\_10 | 1100  |             14.39 |               154 |                 4 |                1 |
| Overload\_Medium\_Thread\_11 | 1097  |             14.43 |               154 |                 5 |                1 |
| Overload\_Medium\_Thread\_12 | 1096  |             14.45 |               155 |                 5 |                1 |
| Overload\_Medium\_Thread\_13 | 1096  |             14.45 |               155 |                 5 |                1 |
| Overload\_Medium\_Thread\_14 | 1095  |             14.47 |               168 |                 5 |                1 |
| Overload\_Medium\_Thread\_15 | 1096  |             13.57 |               168 |                 5 |                1 |
| Overload\_Medium\_Thread\_16 | 1085  |             13.71 |               168 |                 5 |                1 |
| Overload\_Medium\_Thread\_17 | 1087  |             13.68 |               169 |                 5 |                2 |
| Overload\_Medium\_Thread\_18 | 1081  |             13.85 |               170 |                 5 |                2 |
| Overload\_Medium\_Thread\_19 | 1079  |             13.78 |               169 |                 5 |                2 |

------
## 📅 Update History

| Date       | Author         | Description / Notes                     |
|------------|---------------|-----------------------------------------|
| 2025-06-17 | Jang Minsu     | Initial creation in Korean              |
| 2025-07-02 | ChatGPT (GPT-4)| Full comment/documentation translated to English |

---

📅 Document Date: 2025-06-17  
🖋️ Author: Minsu Jang
