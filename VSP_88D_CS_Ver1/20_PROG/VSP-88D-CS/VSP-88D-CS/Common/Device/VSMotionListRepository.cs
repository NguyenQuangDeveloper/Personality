using Newtonsoft.Json;
using System.IO;
using VSLibrary.Controller;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Device;

namespace VSP_88D_CS.Common.Device
{
    public class VSMotionListRepository
    {
        string mtr_path_def = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA", "MTR_DEFINE.json");
        public List<VSMotionListItem> Data { get; private set; } = new List<VSMotionListItem>();
        public VSMotionListRepository()
        {
            InitializeDefaultData();
        }

        public List<VSMotionListItem> GetAll()
        {
            try
            {
                return LoadFromFile(mtr_path_def);
            }
            catch (Exception ex)
            {

            }
            return new List<VSMotionListItem>();
        }

        private void InitializeDefaultData()
        {
            var existingData = GetAll().ToList();

            if (existingData.Count > 0)
            {
                Data = existingData;
                return;
            }
            else
            {
                Data = new List<VSMotionListItem>
                {
                    //new VSMotionListItem {
                    //    MotionController = ControllerType.AIO_AjinAXT,
                    //    AxisNo = (int)eMtr.Sm00_LoadElev_Z,
                    //    AxisName = "MT00",
                    //    StrAxisData = "Loading Elevator Z",
                    //    ServoEnabled = false,
                    //    ServoEnabledReversal = false,
                    //    Position = 0,
                    //    Speed = 100,
                    //    Acceleration = 50,
                    //    HomeState = false,
                    //    MinSpeed = 1.0,
                    //    MaxSpeed = 100,
                    //    LeadPitch = 5,
                    //    PulsesPerRev = 10000,
                    //    GearRatio = 1.0,
                    //    PulseOutputMode = 0x4,
                    //    EncInputMode = 0x3,
                    //    EndLimitP = false,
                    //    EndLimitN = false,
                    //    SlowLimitP = true,
                    //    SlowLimitN = true,
                    //    InPosition = false,
                    //    Alarm = false
                    //},
                    //new VSMotionListItem {
                    //    MotionController = ControllerType.AIO_AjinAXT,
                    //    AxisNo = (int)eMtr.Sm01_LoadPush_X,
                    //    AxisName = "MT01",
                    //    StrAxisData = "Loading Pusher X",
                    //    ServoEnabled = false,
                    //    ServoEnabledReversal = false,
                    //    Position = 0,
                    //    Speed = 100,
                    //    Acceleration = 50,
                    //    HomeState = false,
                    //    MinSpeed = 1.0,
                    //    MaxSpeed = 100,
                    //    LeadPitch = 5,
                    //    PulsesPerRev = 10000,
                    //    GearRatio = 1.0,
                    //    PulseOutputMode = 0x4,
                    //    EncInputMode = 0x3,
                    //    EndLimitP = false,
                    //    EndLimitN = false,
                    //    SlowLimitP = true,
                    //    SlowLimitN = true,
                    //    InPosition = false,
                    //    Alarm = false
                    //},
                    //new VSMotionListItem {
                    //    MotionController = ControllerType.AIO_AjinAXT,
                    //    AxisNo = (int)eMtr.Sm02_Index_X,
                    //    AxisName = "MT02",
                    //    StrAxisData = "Index Pusher X",
                    //    ServoEnabled = false,
                    //    ServoEnabledReversal = false,
                    //    Position = 0,
                    //    Speed = 100,
                    //    Acceleration = 50,
                    //    HomeState = false,
                    //    MinSpeed = 1.0,
                    //    MaxSpeed = 100,
                    //    LeadPitch = 5,
                    //    PulsesPerRev = 10000,
                    //    GearRatio = 1.0,
                    //    PulseOutputMode = 0x4,
                    //    EncInputMode = 0x3,
                    //    EndLimitP = false,
                    //    EndLimitN = false,
                    //    SlowLimitP = true,
                    //    SlowLimitN = true,
                    //    InPosition = false,
                    //    Alarm = false
                    //},
                    //new VSMotionListItem {
                    //    MotionController = ControllerType.AIO_AjinAXT,
                    //    AxisNo = (int)eMtr.Sm03_UnldElev_Z,
                    //    AxisName = "MT03",
                    //    StrAxisData = "Unloading Elevator Z",
                    //    ServoEnabled = false,
                    //    ServoEnabledReversal = false,
                    //    Position = 0,
                    //    Speed = 100,
                    //    Acceleration = 50,
                    //    HomeState = false,
                    //    MinSpeed = 1.0,
                    //    MaxSpeed = 100,
                    //    LeadPitch = 5,
                    //    PulsesPerRev = 10000,
                    //    GearRatio = 1.0,
                    //    PulseOutputMode = 0x4,
                    //    EncInputMode = 0x3,
                    //    EndLimitP = false,
                    //    EndLimitN = false,
                    //    SlowLimitP = true,
                    //    SlowLimitN = true,
                    //    InPosition = false,
                    //    Alarm = false
                    //},new VSMotionListItem {
                    //    MotionController = ControllerType.AIO_AjinAXT,
                    //    AxisNo = (int)eMtr.Sm04_Inspect_Y,
                    //    AxisName = "MT04",
                    //    StrAxisData = "Inspect Y",
                    //    ServoEnabled = false,
                    //    ServoEnabledReversal = false,
                    //    Position = 0,
                    //    Speed = 100,
                    //    Acceleration = 50,
                    //    HomeState = false,
                    //    MinSpeed = 1.0,
                    //    MaxSpeed = 100,
                    //    LeadPitch = 5,
                    //    PulsesPerRev = 10000,
                    //    GearRatio = 1.0,
                    //    PulseOutputMode = 0x4,
                    //    EncInputMode = 0x3,
                    //    EndLimitP = false,
                    //    EndLimitN = false,
                    //    SlowLimitP = true,
                    //    SlowLimitN = true,
                    //    InPosition = false,
                    //    Alarm = false
                    //}
                };
                SaveToFile(mtr_path_def, Data);
            }
        }

        //public void AddMotionList(Dictionary<int, IMotionData> dicdata)
        //{
        //    foreach (var item in dicdata.Values)
        //    {
        //        // item이 VSMotionListItem 타입이 아니라면 새 객체를 생성하여 프로퍼티를 복사합니다.
        //        VSMotionListItem motionItem = item as VSMotionListItem;
        //        if (motionItem == null)
        //        {
        //            motionItem = new VSMotionListItem
        //            {
        //                MotionController = item.MotionController,
        //                AxisNo = item.AxisNo,
        //                AxisName = item.AxisName,
        //                StrAxisData = item.StrAxisData,
        //                ServoEnabled = item.ServoEnabled,
        //                ServoEnabledReversal = item.ServoEnabledReversal,
        //                Position = item.Position,
        //                Speed = item.Speed,
        //                Acceleration = item.Acceleration,
        //                HomeState = item.HomeState,
        //                MinSpeed = item.MinSpeed,
        //                MaxSpeed = item.MaxSpeed,
        //                LeadPitch = item.LeadPitch,
        //                PulsesPerRev = item.PulsesPerRev,
        //                GearRatio = item.GearRatio,
        //                PulseOutputMode = item.PulseOutputMode,
        //                EncInputMode = item.EncInputMode,
        //                EndLimitP = item.LvSettings.EndLimitP,
        //                EndLimitN = item.LvSettings.EndLimitN,
        //                SlowLimitP = item.LvSettings.SlowLimitP,
        //                SlowLimitN = item.LvSettings.SlowLimitN,
        //                InPosition = item.LvSettings.InPosition,
        //                Alarm = item.LvSettings.Alarm
        //                // LvSettings는 필요에 따라 복사할 수 있습니다.
        //            };
        //        }

        //        AddMotionList(motionItem);
        //    }
        //}

        private bool AddMotionList(VSMotionListItem motionList)
        {
            Data.Add(motionList);
            SaveToFile(mtr_path_def, Data);
            return true;
        }

        public bool DeleteMotionList(int AxisNo)
        {
            //var mtrToRemove = Data.FirstOrDefault(x => x.AxisNo == AxisNo);
            //if (mtrToRemove != null)
            //{
            //    Data.Remove(mtrToRemove);
            //}
            //SaveToFile(mtr_path_def, Data);
            return true;
        }

        //public bool UpdateMotion(IMotionData motiondata)
        //{
        //    throw new NotImplementedException();
        //}

        public List<VSMotionListItem> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<VSMotionListItem>();

            var json = File.ReadAllText(filePath);
            return JsonHelper.SafeDeserializeJSON<List<VSMotionListItem>>(json);
        }

        public void SaveToFile(string filePath, List<VSMotionListItem> data)
        {
            var json = JsonHelper.SafeSerializeJSON(data);
            string directoryFolder = Directory.GetParent(filePath)!.FullName;
            if (!Directory.Exists(directoryFolder))
            {
                Directory.CreateDirectory(directoryFolder);
            }
            File.WriteAllText(filePath, json);
        }
    }
}
