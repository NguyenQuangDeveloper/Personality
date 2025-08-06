using Mysqlx.Crud;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Controller.AnalogIO;
using VSLibrary.Controller.DigitalIO;
using VSLibrary.Controller.Motion;
using VSLibrary.Threading;

namespace VSLibrary.Controller
{
    /// <summary>
    /// Manages all controllers: analog I/O, digital I/O, and motion axes.
    /// Aggregates data dictionaries and handles periodic updates via a worker thread.
    /// </summary>
    public class ControllerManager
    {
        // Dictionaries for channel-based (AIO), bit-based (DIO), and axis-based (Motion) data
        public Dictionary<string, IAnalogIOData> AIOData = new Dictionary<string, IAnalogIOData>();
        public Dictionary<string, IDigitalIOData> DIOData = new Dictionary<string, IDigitalIOData>();
        public Dictionary<int, IAxisData> AxisData = new Dictionary<int, IAxisData>();

        // Counters for assigning I/O indices across different controllers
        public Dictionary<string, int> count = new Dictionary<string, int>()
        {
            { "AInput", 0 },
            { "AOutput", 0 },
            { "DInput", 0 },
            { "DOutput", 0 },
            { "MDInput", 0 },
            { "MDOutput", 0 },
        };

        /// <summary>
        /// Initializes controllers based on requested types, populates data dictionaries, and starts the update thread.
        /// </summary>
        /// <param name="container">Dependency injection container for registering instances.</param>
        /// <param name="Controllerlist">List of controller types to initialize.</param>
        public ControllerManager(IContainer container, List<ControllerType> Controllerlist)
        {
            foreach (var controllerType in Controllerlist)
            {
                // Initialize AXT library if any Ajin AXT controllers are requested
                if ((controllerType == ControllerType.AIO_AjinAXT)
                    || (controllerType == ControllerType.DIO_AjinAXT)
                    || (controllerType == ControllerType.Motion_AjinAXT))
                    LibraryInitializer.InitializeAxt();

                // Initialize Adlink DASK library
                if (controllerType == ControllerType.AIO_Adlink)
                    LibraryInitializer.InitializeAdlink();

                switch (controllerType)
                {
                    //count로 각 컨트롤러의 I/O 범위를 정할수 있음.
                    case ControllerType.AIO_AjinAXT:
                        var axtAIOCtrl = new AjinAxtAIO(count);

                        foreach (var data in axtAIOCtrl.GetAnalogIODataDictionary())
                        {
                            AIOData[data.Key] = data.Value;
                        }
                        break;

                    case ControllerType.AIO_Adlink:
                        var adlinkAIOCtrl = new ADLinkAIO(count);

                        foreach (var data in adlinkAIOCtrl.GetAnalogIODataDictionary())
                        {
                            AIOData[data.Key] = data.Value;
                        }
                        break;

                    case ControllerType.DIO_AjinAXT:
                        var axtDIOCtrl = new AjinAxtDIO(count);

                        foreach (var data in axtDIOCtrl.GetDigitalIODataDictionary())
                        {
                            DIOData[data.Key] = data.Value;
                        }
                        break;

                    case ControllerType.DIO_Comizoa:
                        var comizoaDIOCtrl = new ComizoaDIO(count);

                        foreach (var data in comizoaDIOCtrl.GetDigitalIODataDictionary())
                        {
                            DIOData[data.Key] = data.Value;
                        }
                        break;

                    case ControllerType.Motion_AjinAXT:
                        var AxtMotionCtrl = new AxtMotion(count);

                        foreach (var data in AxtMotionCtrl.GetDigitalIODataDictionary())
                        {
                            DIOData[data.Key] = data.Value;
                        }

                        foreach (var data in AxtMotionCtrl.GetMotionDataDictionary())
                        {
                            AxisData[data.Key] = data.Value;
                        }

                        break;
                        // 다른 컨트롤러 유형도 추가 가능
                }
            }

            // Start a high-priority worker thread for periodic data updates
            ThreadManager.CreateVirtualThread("SampleThread", WorkLoop, ThreadPriorityLevel.High, 0);//@"Threading\MainWindowViewModel.txt");

            //UpdateControllerData();
        }

        /// <summary>
        /// Worker thread loop invoking controller updates.
        /// </summary>
        /// <param name="count">Iteration counter (unused).</param>
        private void WorkLoop(int count)
        {
            UpdateControllerData();
        }

        /// <summary>
        /// Updates all controller data: analog channels, digital I/O, and motion statuses/positions.
        /// </summary>
        private void UpdateControllerData()
        {
            try
            {
                // Update analog I/O controllers
                var aioControllers = AIOData.Values
                    .Where(d => d.Controller != null)
                    .Select(d => d.Controller)
                    .Distinct()
                    .ToList();

                foreach (var aio in aioControllers)
                {
                    aio.UpdateAllChannelValues();
                }

                // Update digital I/O controllers
                var dioControllers = DIOData.Values
                    .Where(d => d.Controller != null)
                    .Select(d => d.Controller)
                    .Distinct()
                    .ToList();

                foreach (var dio in dioControllers)
                {
                    dio.UpdateAllIOStates();
                }

                // Update motion controllers
                var motionControllers = AxisData.Values
                    .Where(d => d.Controller != null)
                    .Select(d => d.Controller)
                    .Distinct()
                    .ToList();

                foreach (var motion in motionControllers)
                {
                    motion.UpdateAllIOStatus();
                    motion.UpdateAllPosition();
                }
            }
            catch 
            {
                //LogManager.Write($"ControllerManager WorkLoop Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies user-defined I/O settings to existing or new analog/digital I/O data objects.
        /// </summary>
        /// <param name="iODataList">List of I/O setting entries.</param>
        public void SetIOlist(List<IIOSettinglist> iODataList)
        {
            if (iODataList == null) return;

            foreach (var setting in iODataList)
            {
                if (string.IsNullOrWhiteSpace(setting.WireName)) continue;

                var key = setting.WireName;

                // Analog I/O
                if (key.StartsWith("A"))
                {
                    if (AIOData.TryGetValue(key, out var aio))
                    {
                        aio.IOType = setting.IOType;
                        aio.EmName = setting.EmName;
                        aio.StrdataName = setting.StrdataName;
                    }
                    else
                    {
                        // 새 AIOData 생성 필요 시: 기본 구현체 사용
                        var newAio = new AIOData();

                        newAio.IOType = setting.IOType;
                        newAio.WireName = setting.WireName;
                        newAio.EmName = setting.EmName;
                        newAio.StrdataName = setting.StrdataName;

                        AIOData[key] = newAio;
                    }
                }
                // Digital I/O (X, Y, M prefixes)
                else if (key.StartsWith("X") || key.StartsWith("Y") || key.StartsWith("M"))
                {
                    if (DIOData.TryGetValue(key, out var dio))
                    {
                        dio.IOType = setting.IOType;
                        dio.EmName = setting.EmName;
                        dio.StrdataName = setting.StrdataName;
                    }
                    else
                    {
                        var newDio = new DIOData();

                        newDio.IOType = setting.IOType;
                        newDio.WireName = setting.WireName;
                        newDio.EmName = setting.EmName;
                        newDio.StrdataName = setting.StrdataName;

                        DIOData[key] = newDio;
                    }
                }
            }
        }

        /// <summary>
        /// Applies user-defined axis settings; updates existing IAxisData or creates new AxtAxisData entries.
        /// </summary>
        /// <param name="axisList">List of axis setting entries.</param>
        public void SetAxislist(List<IAxisSettinglist> axisList)
        {
            if (axisList == null) return;

            foreach (var setting in axisList)
            {
                var axisNo = setting.AxisNo;
                if (AxisData.TryGetValue(axisNo, out var axis))
                {
                    // Update existing axis data
                    axis.AxisName = setting.AxisName;
                    axis.StrAxisData = setting.StrAxisData;
                    axis.LeadPitch = setting.LeadPitch;
                    axis.PulsesPerRev = setting.PulsesPerRev;
                    axis.GearRatio = setting.GearRatio;
                    axis.PulseOutputMode = setting.PulseOutputMode;
                    axis.EncInputMode = setting.EncInputMode;
                    axis.MinSpeed = setting.MinSpeed;
                    axis.MaxSpeed = setting.MaxSpeed;
                    axis.ServoEnabledReversal = setting.ServoEnabledReversal;

                    axis.LvSet_EndLimitP = setting.LvSet_EndLimitP;
                    axis.LvSet_EndLimitN = setting.LvSet_EndLimitN;
                    axis.LvSet_SlowLimitP = setting.LvSet_SlowLimitP;
                    axis.LvSet_SlowLimitN = setting.LvSet_SlowLimitN;
                    axis.LvSet_InPosition = setting.LvSet_InPosition;
                    axis.LvSet_Alarm = setting.LvSet_Alarm;

                    axis.Controller.SetParameter(axis);
                }
                else
                {
                    // Create new axis data with default AxtAxisData class
                    var newAxis = new AxtAxisData
                    {
                        AxisName = setting.AxisName,
                        StrAxisData = setting.StrAxisData,
                        LeadPitch = setting.LeadPitch,
                        PulsesPerRev = setting.PulsesPerRev,
                        GearRatio = setting.GearRatio,
                        PulseOutputMode = setting.PulseOutputMode,
                        EncInputMode = setting.EncInputMode,
                        MinSpeed = setting.MinSpeed,
                        MaxSpeed = setting.MaxSpeed,
                        ServoEnabledReversal = setting.ServoEnabledReversal,
                        LvSet_EndLimitP = setting.LvSet_EndLimitP,
                        LvSet_EndLimitN = setting.LvSet_EndLimitN,
                        LvSet_SlowLimitP = setting.LvSet_SlowLimitP,
                        LvSet_SlowLimitN = setting.LvSet_SlowLimitN,
                        LvSet_InPosition = setting.LvSet_InPosition,
                        LvSet_Alarm = setting.LvSet_Alarm
                    };

                    AxisData[axisNo] = newAxis;
                }
            }
        }
    }

    /// <summary>
    /// Initializes native libraries (Ajin AXT and Adlink DASK) by setting DLL directories and calling native entry points.
    /// </summary>
    public class LibraryInitializer
    {
        public static bool _initLibraryAxt = false;
        public static bool _initLibraryAdlink = false;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        /// Initializes the Ajin AXT native library, sets DLL directory, and opens the device if not already initialized.
        /// </summary>
        /// <returns>True if initialization succeeded or was already done; false otherwise.</returns>
        public static bool InitializeAxt()
        {
            if (_initLibraryAxt) return _initLibraryAxt;

            string dllPath = $@"3rd Party Files\Controller\Ajin\AxtLib";
            if (!SetDllDirectory(dllPath))
            {
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"[Ajin] Failed to set DLL directory: {dllPath}, Error: {errorCode}");
                return false;
            }

            if (CAxtLib.AxtIsInitialized() == 0)
            {
                if (CAxtLib.AxtInitialize(IntPtr.Zero, 0) == 0)
                {
                    Console.WriteLine("[Ajin] AxtInitialize failed");
                    return false;
                }
            }

            if (CAxtLib.AxtIsInitializedBus(1) == 0)
            {
                if (CAxtLib.AxtOpenDeviceAuto(1) == 0)
                {
                    Console.WriteLine("[Ajin] AxtOpenDeviceAuto failed");
                    return false;
                }
            }

            Console.WriteLine("[Ajin] AXT library initialization complete");

            _initLibraryAxt = true;

            return _initLibraryAxt;
        }

        /// <summary>
        /// Initializes the Adlink DASK native library by setting the DLL directory.
        /// </summary>
        /// <returns>True if initialization succeeded or was already done; false otherwise.</returns>
        public static bool InitializeAdlink()
        {
            if (_initLibraryAdlink) return _initLibraryAdlink;

            string dllPath = $@"3rd Party Files\Controller\AdlinkLib\Dask\AdlinkLib";
            if (!SetDllDirectory(dllPath))
            {
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine("[Adlink] Failed to set DLL directory: {dllPath}, Error: {errorCode}");
                return false;
            }

            Console.WriteLine("[Adlink] DASK library initialization complete");
            _initLibraryAdlink = true;

            return _initLibraryAdlink;
        }
    }
}
