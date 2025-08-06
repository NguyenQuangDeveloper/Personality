using Newtonsoft.Json;
using System.IO;
using VSP_88D_CS.Models.Device;

namespace VSP_88D_CS.Common.Device
{
    public class VSIOSettingRepository
    {   
        string io_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA", "IO_DEFINE.json");
        public List<VSIOSettingItem> Data { get; set; }
        public VSIOSettingRepository()
        {
            InitializeDefaultData();
        }

        public List<VSIOSettingItem> GetAll()
        {
            try
            {
                return LoadFromFile(io_path);
            }
            catch (Exception ex)
            {
                
            }
            return new List<VSIOSettingItem>();
        }

        private void InitializeDefaultData()
        {
            var existingData = GetAll().ToList();

            if (existingData.Any())
            {
                Data = existingData;
                return;
            }

            Data = new List<VSIOSettingItem>
            {
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X000", EmName = "I_EmgSwFront", StrdataName = "Emergency Switch(Front)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X001", EmName = "I_EmgSwRear", StrdataName = "Emergency Switch(Rear)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X002", EmName = "I_DoorFrontLeft", StrdataName = "Safety Door(Front Left)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X003", EmName = "I_DoorFrontCenter", StrdataName = "Safety Door(Front Center)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X004", EmName = "I_DoorFrontRight", StrdataName = "Safety Door(Front Right)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X005", EmName = "I_DoorLeft", StrdataName = "Safety Door(Left)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X006", EmName = "I_DoorRight", StrdataName = "Safety Door(Right)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X007", EmName = "I_PumpOvld_Pm1", StrdataName = "Pump Overload"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X008", EmName = "I_MainAirPressure", StrdataName = "Main Air Pressure"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X009", EmName = "I_Gas1Pressure", StrdataName = "Gas1 Pressure"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00A", EmName = "I_Gas2Pressure", StrdataName = "Gas2 Pressure"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00B", EmName = "I_VacValveOpen_Pm1", StrdataName = "Vacuum Valve Open"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00C", EmName = "I_ECoolOn_Pm1", StrdataName = "Electrode Cooling"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00D", EmName = "I_MCoolOn_Pm1", StrdataName = "Matcher Cooling"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00E", EmName = "I_ChamberUp_Pm1", StrdataName = "Chamber Open"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X00F", EmName = "I_ChamberDn_Pm1", StrdataName = "Chamber Close"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X010", EmName = "I_PumpOilLevel_Pm1", StrdataName = "Pump Oil Check"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X011", EmName = "I_LoadElevMgzChk_1", StrdataName = "Load Elevator Mgz. Detect #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X012", EmName = "I_LoadElevMgzChk_2", StrdataName = "Load Elevator Mgz. Detect #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X013", EmName = "I_LoadElevMgzChk_3", StrdataName = "Load Elevator Mgz. Detect #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X014", EmName = "I_LoadElevMgzChk_4", StrdataName = "Load Elevator Mgz. Detect #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X015", EmName = "I_Pusher1Overload", StrdataName = "Pusher #1 Overload"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X016", EmName = "I_LoadElevStripChk_1", StrdataName = "Load Elevator Strip Detect #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X017", EmName = "I_LoadElevStripChk_2", StrdataName = "Load Elevator Strip Detect #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X018", EmName = "I_LoadElevStripChk_3", StrdataName = "Load Elevator Strip Detect #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X019", EmName = "I_LoadElevStripChk_4", StrdataName = "Load Elevator Strip Detect #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01A", EmName = "I_InBufStopperUp", StrdataName = "Loading Stopper Up"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01B", EmName = "I_InBufStopperDn", StrdataName = "Loading Stopper Down"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01C", EmName = "I_InBufDevArrival_1", StrdataName = "Load Station Strip Arrival #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01D", EmName = "I_InBufDevArrival_2", StrdataName = "Load Station Strip Arrival #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01E", EmName = "I_InBufDevArrival_3", StrdataName = "Load Station Strip Arrival #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X01F", EmName = "I_InBufDevArrival_4", StrdataName = "Load Station Strip Arrival #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X020", EmName = "I_InBufCylFwd", StrdataName = "Load Station Fwd.(Right)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X021", EmName = "I_InBufCylBwd", StrdataName = "Laod Station Bwd.(Left)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X022", EmName = "I_Pusher2Overload", StrdataName = "Pusher #2 Overload"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X023", EmName = "I_Pusher2StripChk_1", StrdataName = "Pusher #2 Strip Check #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X024", EmName = "I_Pusher2StripChk_2", StrdataName = "Pusher #2 Strip Check #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X025", EmName = "I_Pusher2StripChk_3", StrdataName = "Pusher #2 Strip Check #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X026", EmName = "I_Pusher2StripChk_4", StrdataName = "Pusher #2 Strip Check #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X027", EmName = "I_Pusher2Up", StrdataName = "Pusher #2 Up Pos."},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X028", EmName = "I_Pusher2Dn", StrdataName = "Pusher #2 Down Pos."},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X029", EmName = "I_OutBufFwd", StrdataName = "Unload Station Fwd.(Right)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02A", EmName = "I_OutBufBwd", StrdataName = "Unload Station Bwd.(Left)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02B", EmName = "I_ChamberOutStripChk_1", StrdataName = "Chamber Out Strip Check #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02C", EmName = "I_ChamberOutStripChk_2", StrdataName = "Chamber Out Strip Check #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02D", EmName = "I_ChamberOutStripChk_3", StrdataName = "Chamber Out Strip Check #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02E", EmName = "I_ChamberOutStripChk_4", StrdataName = "Chamber Out Strip Check #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X02F", EmName = "I_Pusher3Overload", StrdataName = "Pusher #3 Overload"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X030", EmName = "I_Pusher3Up", StrdataName = "Pusher #3 Up Pos."},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X031", EmName = "I_Pusher3Dn", StrdataName = "Pusher #3 Down Pos."},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X032", EmName = "I_Pusher3Fwd", StrdataName = "Pusher #3 Fwd.(Right)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X033", EmName = "I_Pusher3Bwd", StrdataName = "Pusher #3 Bwd.(Left)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X034", EmName = "I_Pusher3DnEnable_1", StrdataName = "Pusher #3 Down Enable Check #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X035", EmName = "I_Pusher3DnEnable_2", StrdataName = "Pusher #3 Down Enable Check #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X036", EmName = "I_Pusher3DnEnable_3", StrdataName = "Pusher #3 Down Enable Check #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X037", EmName = "I_Pusher3DnEnable_4", StrdataName = "Pusher #3 Down Enable Check #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X038", EmName = "I_UnLoadElevStripChk_1", StrdataName = "Unload Elevator Strip Detect #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X039", EmName = "I_UnLoadElevStripChk_2", StrdataName = "Unload Elevator Strip Detect #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03A", EmName = "I_UnLoadElevStripChk_3", StrdataName = "Unload Elevator Strip Detect #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03B", EmName = "I_UnLoadElevStripChk_4", StrdataName = "Unload Elevator Strip Detect #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03C", EmName = "I_UnLoadElevMgzChk_1", StrdataName = "Unload Elevator Mgz. Detect #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03D", EmName = "I_UnLoadElevMgzChk_2", StrdataName = "Unload Elevator Mgz. Detect #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03E", EmName = "I_UnLoadElevMgzChk_3", StrdataName = "Unload Elevator Mgz. Detect #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X03F", EmName = "I_UnLoadElevMgzChk_4", StrdataName = "Unload Elevator Mgz. Detect #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X040", EmName = "I_VisionOut", StrdataName = "Vision Output"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X041", EmName = "I_VisionEnable", StrdataName = "Vision Enable"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X042", EmName = "I_VisionError", StrdataName = "Vision Error"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X043", EmName = "I_Gas3Pressure", StrdataName = "Gas3 Pressure"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X044", EmName = "I_Gas4Pressure", StrdataName = "Gas4 Pressure"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X045", EmName = "I_Pusher2Overload2", StrdataName = "Pusher #2 Overload #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X046", EmName = "I_Pusher2Overload3", StrdataName = "Pusher #2 Overload #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X047", EmName = "I_Pusher2Overload4", StrdataName = "Pusher #2 Overload #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X048", EmName = "I_LoadElevMgzChk_5", StrdataName = "Load Elevator Mgz. Detect #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X049", EmName = "I_LoadElevStripChk_5", StrdataName = "Load Elevator Strip Detect #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04A", EmName = "I_InBufDevArrival_5", StrdataName = "Load Station Strip Arrival #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04B", EmName = "I_Pusher2StripChk_5", StrdataName = "Pusher #2 Strip Check #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04C", EmName = "I_ChamberOutStripChk_5", StrdataName = "Chamber Out Strip Check #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04D", EmName = "I_Pusher3DnEnable_5", StrdataName = "Pusher #3 Down Enable Check #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04E", EmName = "I_UnLoadElevStripChk_5", StrdataName = "Unload Elevator Strip Detect #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X04F", EmName = "I_UnLoadElevMgzChk_5", StrdataName = "Unload Elevator Mgz. Detect #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X050", EmName = "I_LoadMgzTiltChk_1", StrdataName = "Load Elevator Mgz. Tilt Check #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X051", EmName = "I_LoadMgzTiltChk_2", StrdataName = "Load Elevator Mgz. Tilt Check #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X052", EmName = "I_LoadMgzTiltChk_3", StrdataName = "Load Elevator Mgz. Tilt Check #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X053", EmName = "I_LoadMgzTiltChk_4", StrdataName = "Load Elevator Mgz. Tilt Check #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X054", EmName = "I_UnldMgzTiltChk_1", StrdataName = "Unload Elevator Mgz. Tilt Check #1"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X055", EmName = "I_UnldMgzTiltChk_2", StrdataName = "Unload Elevator Mgz. Tilt Check #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X056", EmName = "I_UnldMgzTiltChk_3", StrdataName = "Unload Elevator Mgz. Tilt Check #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X057", EmName = "I_UnldMgzTiltChk_4", StrdataName = "Unload Elevator Mgz. Tilt Check #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X058", EmName = "I_IonizerAlarm", StrdataName = "Ionizer Alarm"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X059", EmName = "I_LaneFloatDetect", StrdataName = "Floating Lane Detect"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05A", EmName = "I_Pusher1Overload2", StrdataName = "Pusher #1 Overload #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05B", EmName = "I_Pusher1Overload3", StrdataName = "Pusher #1 Overload #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05C", EmName = "I_Pusher1Overload4", StrdataName = "Pusher #1 Overload #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05D", EmName = "I_Pusher1Overload5", StrdataName = "Pusher #1 Overload #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05E", EmName = "I_Pusher2Overload5", StrdataName = "Pusher #2 Overload #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X05F", EmName = "I_ChamberInEdge", StrdataName = "Chamber In Strip Detect"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X060", EmName = "I_ChamberOutEdge", StrdataName = "Chamber Out Strip Detect"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X061", EmName = "I_Pusher3Overload2", StrdataName = "Pusher #3 Overload #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X062", EmName = "I_Pusher3Overload3", StrdataName = "Pusher #3 Overload #3"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X063", EmName = "I_Pusher3Overload4", StrdataName = "Pusher #3 Overload #4"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X064", EmName = "I_Pusher3Overload5", StrdataName = "Pusher #3 Overload #5"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X065", EmName = "I_N2Pressure_CJ", StrdataName = "N2 Pressure(CJ)"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X066", EmName = "I_LoadElevMgzTiltCheck", StrdataName = "Load Elevator Mgz. Tilt Check"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X067", EmName = "I_UnldElevMgzTiltCheck", StrdataName = "Unload Elevator Mgz. Tilt Check"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X068", EmName = "I_IonizerAlarm2", StrdataName = "Ionizer Alarm #2"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X069", EmName = "I_X069", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06A", EmName = "I_X06A", StrdataName = "Safety ON"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06B", EmName = "I_X06B", StrdataName = "Safety Alarm"},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06C", EmName = "I_X06C", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06D", EmName = "I_X06D", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06E", EmName = "I_X06E", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.InPut, WireName = "X06F", EmName = "I_X06F", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y000", EmName = "O_TowerRed", StrdataName = "Tower Lamp Red"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y001", EmName = "O_TowerGreen", StrdataName = "Tower Lamp Green"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y002", EmName = "O_TowerYellow", StrdataName = "Tower Lamp Yellow"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y003", EmName = "O_Buzzer", StrdataName = "Buzzer"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y004", EmName = "O_RfPwrOn", StrdataName = "RF Generator Power"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y005", EmName = "O_VPumpOn", StrdataName = "Vacuum Pump Power"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y006", EmName = "O_VValOpen", StrdataName = "Vacuum Valve Ope"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y007", EmName = "O_GValOpen", StrdataName = "Gauge Protection Valve Open"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y008", EmName = "O_Mfc1Open", StrdataName = "Gas1 Injection"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y009", EmName = "O_Mfc2Open", StrdataName = "Gas2 Injection"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00A", EmName = "O_ECoolOn", StrdataName = "Electrode Cooling"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00B", EmName = "O_MCoolOn", StrdataName = "Matcher Cooling"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00C", EmName = "O_N2Purge", StrdataName = "N2 Purge"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00D", EmName = "O_AirPurge", StrdataName = "Air Purge"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00E", EmName = "O_RfOn", StrdataName = "Electrode Cooling(TOP)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y00F", EmName = "O_ServoPwrOn", StrdataName = "Electrode Cooling(BOTTOM)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y010", EmName = "O_ChamberUp", StrdataName = "Matcher Cooling(TOP)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y011", EmName = "O_ChamberDn", StrdataName = "Matcher Cooling(BOTTOM)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y012", EmName = "O_InBufStopperUp", StrdataName = "RF On Timer"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y013", EmName = "O_InBufStopperDn", StrdataName = "Servo Power"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y014", EmName = "O_InBufFwd", StrdataName = "Lamp On"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y015", EmName = "O_InBufBwd", StrdataName = "Chamber Open"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y016", EmName = "O_Pusher2Up", StrdataName = "Chamber Close"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y017", EmName = "O_Pusher2Dn", StrdataName = "Bridge Up"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y018", EmName = "O_OutBufFwd", StrdataName = "Unload Station Fwd.(Right)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y019", EmName = "O_OutBufBwd", StrdataName = "Unload Station Bwd.(Left)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01A", EmName = "O_Pusher3Up", StrdataName = "Pusher #3 Up"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01B", EmName = "O_Pusher3Dn", StrdataName = "Pusher #3 Down"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01C", EmName = "O_Pusher3Fwd", StrdataName = "Pusher #3 Fwd.(Right)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01D", EmName = "O_Pusher3Bwd", StrdataName = "Pusher #3 Bwd.(Left)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01E", EmName = "O_NC", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y01F", EmName = "O_NC1", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y020", EmName = "O_InBufRollRun", StrdataName = "Loading Roller Run"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y021", EmName = "O_InBufRollSpd", StrdataName = "Loading Roller Speed Selection"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y022", EmName = "O_OutBufRollRun", StrdataName = "Unloading Roller Run"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y023", EmName = "O_VisionTeach", StrdataName = "Vision Teaching"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y024", EmName = "O_VisionTrig", StrdataName = "Vision Trigger"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y025", EmName = "O_VisionBank1", StrdataName = "Vision Bank #1"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y026", EmName = "O_VisionBank2", StrdataName = "Vision Bank #2"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y027", EmName = "O_VisionBank3", StrdataName = "Vision Bank #3"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y028", EmName = "O_Mfc3Open", StrdataName = "Gas3 Injection(Option)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y029", EmName = "O_Mfc4Open", StrdataName = "Gas4 Injection(Option)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02A", EmName = "Vision Reset", StrdataName = "Vision Reset"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02B", EmName = "O_NC8", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02C", EmName = "O_NC9", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02D", EmName = "O_NC10", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02E", EmName = "O_NC11", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y02F", EmName = "O_NC12", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y030", EmName = "O_IonizerOn", StrdataName = "Ionizer On"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y031", EmName = "O_IonizerOn2", StrdataName = "Ionizer #2 On"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y032", EmName = "O_DoorUnlockFrontCenter", StrdataName = "Safety Door Unlock(Front Center)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y033", EmName = "O_DoorUnlockFrontRight", StrdataName = "Safety Door Unlock(Front Right)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y034", EmName = "O_DoorUnlockLeft", StrdataName = "Safety Door Unlock(Left)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y035", EmName = "O_DoorUnlockRight", StrdataName = "Safety Door Unlock(Right)"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y036", EmName = "O_Y036", StrdataName = "Bypass Safety"},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y037", EmName = "O_Y037", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y038", EmName = "O_Y038", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y039", EmName = "O_Y039", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03A", EmName = "O_Y03A", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03B", EmName = "O_Y03B", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03C", EmName = "O_Y03C", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03D", EmName = "O_Y03D", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03E", EmName = "O_Y03E", StrdataName = ""},
                //new VSIOSettingItem { IOType = IOType.OUTPut, WireName = "Y03F", EmName = "O_Y03F", StrdataName = ""}
            };
            SaveToFile(io_path, Data);
        }

        public List<VSIOSettingItem> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<VSIOSettingItem>(); 

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<VSIOSettingItem>>(json) ?? new List<VSIOSettingItem>();
        }

        public void SaveToFile(string filePath, List<VSIOSettingItem> data)
        {
            var json = JsonConvert.SerializeObject(data);

            string directory = Directory.GetParent(filePath)!.FullName;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }

        private bool Exists(string wireName)
        {
            //return Data.Any(item => item.WireName == wireName);
            return true;
        }

        public VSIOSettingItem GetIOSetting(int id)
        {
            //return Data.FirstOrDefault(x => x.Id == id);
            return null;
        }
        public IEnumerable<VSIOSettingItem> GetAllIOSetting()
        {
            return GetAll();
        }
        public async Task<bool> UpdateIOItem(VSIOSettingItem IOSettingItem)
        {
            //return await _databaseManager.UpdateAsync<VSIOSettingItem>(IOSettingItem);
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteIOItem(VSIOSettingItem IOSettingItem)
        {
            //return await _databaseManager.DeleteAsync<VSIOSettingItem>(IOSettingItem);
            throw new NotImplementedException();
        }
    }
}
