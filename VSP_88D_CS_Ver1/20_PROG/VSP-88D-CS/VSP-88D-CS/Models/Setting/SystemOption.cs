using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting
{
    public enum eSystemOptType
    {
        MFC = 0,
        RF_GEN,
        VAC_GAUGE,
        VAC_PUMP,
        ETC
    }
    public class SystemOption : ViewModelBase
    {
        #region PROPERTY
        private ObservableCollection<ParameterItem> _parameterList;
        public ObservableCollection<ParameterItem> ParameterList
        {
            get => _parameterList;
            set => SetProperty(ref _parameterList, value);
        }

        #endregion PROPERY

        #region FUNCTION
        public SystemOption() 
        {
            _parameterList = new ObservableCollection<ParameterItem>();
        }

        public int GetBuzzerOffTime()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "BUZZER_OFF_TIME");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public bool GetServoPositionChangeEnable()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "SERVO_POSITION_CHANGE_ENABLE");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public bool GetIndexPusherUpAfterLoading()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "INDEX_PUSHER_UP_AFTER_LOADING");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public int GetLanguage()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "LANGUAGE");
            //JsonElement jsonElement = (JsonElement)item.Value;
            //value = jsonElement.GetInt32();
            value = Convert.ToInt32(item.Value.ToString());
            return value;
        }

        public bool GetDoorSensorSkipDuringManual()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "DOOR_SENSOR_SKIP_DURING_MANUAL");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public bool GetUnloadingBufferKeepRight()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "UNLOADING_BUFFER_KEEP_RIGHT");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public bool GetChamberCloseAfterLotEnd()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "CHAMBER_CLOSE_AFTER_LOT_END");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public bool GetUnloadSensorTypeNomalClose()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "UNLOAD_SENSOR_TYPE_NORMAL_CLOSE");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public int GetAutomation()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "AUTOMATION");
            value = Convert.ToInt32(item.Value.ToString());
            return value;
        }

        public int GetGasInjectionWhenVacuumStart()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "GAS_INJECTION_WHEN_VACUUM_START");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public bool GetIdReadingDelay()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "ID_READING_DELAY");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public int GetIdReadingDelayValue()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "ID_READING_DELAY_VALUE");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public bool GetUseIonizer()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "USE_IONIZER");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public bool GetGasUse(int nGasIdx)
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == $"USE_GAS_{nGasIdx}");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public int GetMFCCap(int nGasIdx)
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == $"MFC_CAPACITY_{nGasIdx}");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public string GetGasName(int nGasIdx)
        {
            string value = string.Empty;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == $"GAS_NAME_{nGasIdx}");
            value = Convert.ToString(item.Value);
            return value;
        }

        public bool GetSkipDoorAlarm()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "DOOR_ALARM_SKIP");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public void Default()
        {
            ParameterItem sysItem1 = new ParameterItem();
            sysItem1.Parameter = "BUZZER OFF TIME (3~60)";
            sysItem1.Value = 3;
            sysItem1.Unit = "sec.";
            sysItem1.Tooltip = "Input the BUZZER OFF TIME (3~60) value.";
            sysItem1.IsVisible = true;
            sysItem1.Section = "SYSTEM";
            sysItem1.Key = "BUZZER_OFF_TIME";
            sysItem1.Type = ParameterType.Number;
            _parameterList.Add(sysItem1);

            ParameterItem sysItem2 = new ParameterItem();
            sysItem2.Parameter = "OUT STRIP EMPTY CHECK TIME";
            sysItem2.Value = 500;
            sysItem2.Unit = "msec.";
            sysItem2.Tooltip = "Input the OUT STRIP EMPTY CHECK TIME value.";
            sysItem2.IsVisible = true;
            sysItem2.Section = "SYSTEM";
            sysItem2.Key = "OUT_STRIP_EMPTY_CHECK_TIME";
            sysItem2.Type = ParameterType.Number;
            _parameterList.Add(sysItem2);

            ParameterItem sysItem3 = new ParameterItem();
            sysItem3.Parameter = "SERVO POSITION CHANGE ENABLE";
            sysItem3.Value = false;
            sysItem3.Tooltip = "Input the SERVO POSITION CHANGE ENABLE value.";
            sysItem3.Type = ParameterType.CheckBox;
            sysItem3.IsVisible = true;
            sysItem3.IsEditable = true;
            sysItem3.Section = "SYSTEM";
            sysItem3.Key = "SERVO_POSITION_CHANGE_ENABLE";
            _parameterList.Add(sysItem3);

            ParameterItem sysItem4 = new ParameterItem();
            sysItem4.Parameter = "INDEX PUSHER UP AFTER LOADING";
            sysItem4.Value = false;
            sysItem4.Tooltip = "Input the INDEX PUSHER UP AFTER LOADING value.";
            sysItem4.Type = ParameterType.CheckBox;
            sysItem4.IsVisible = true;
            sysItem4.IsEditable = true;
            sysItem4.Section = "SYSTEM";
            sysItem4.Key = "INDEX_PUSHER_UP_AFTER_LOADING";
            _parameterList.Add(sysItem4);

            ParameterItem sysItem5 = new ParameterItem();
            sysItem5.Parameter = "LANGUAGE";
            sysItem5.Value = 1;
            sysItem5.Tooltip = "Select the LANGUAGE value.";
            sysItem5.Type = ParameterType.ComboBox;
            sysItem5.ComboBoxItems.Clear();
            sysItem5.ComboBoxItems.Add("KOREAN");
            sysItem5.ComboBoxItems.Add("ENGLISH");
            sysItem5.ComboBoxItems.Add("CHINESE");
            sysItem5.IsVisible = true;
            sysItem5.IsEditable = true;
            sysItem5.Section = "SYSTEM";
            sysItem5.Key = "LANGUAGE";
            _parameterList.Add(sysItem5);

            ParameterItem sysItem6 = new ParameterItem();
            sysItem6.Parameter = "DOOR SENSOR SKIP DURING MANUAL";
            sysItem6.Value = false;
            sysItem6.Tooltip = "Select the DOOR SENSOR SKIP DURING MANUAL value.";
            sysItem6.Type = ParameterType.CheckBox;
            sysItem6.IsVisible = true;
            sysItem6.IsEditable = true;
            sysItem6.Section = "SYSTEM";
            sysItem6.Key = "DOOR_SENSOR_SKIP_DURING_MANUAL";
            _parameterList.Add(sysItem6);

            ParameterItem sysItem7 = new ParameterItem();
            sysItem7.Parameter = "UNLOADING BUFFER KEEP RIGHT";
            sysItem7.Value = false;
            sysItem7.Tooltip = "Select the UNLOADING BUFFER KEEP RIGHT value.";
            sysItem7.Type = ParameterType.CheckBox;
            sysItem7.IsVisible = true;
            sysItem7.IsEditable = true;
            sysItem7.Section = "SYSTEM";
            sysItem7.Key = "UNLOADING_BUFFER_KEEP_RIGHT";
            _parameterList.Add(sysItem7);

            ParameterItem sysItem8 = new ParameterItem();
            sysItem8.Parameter = "CHAMBER CLOSE AFTER LOT END";
            sysItem8.Value = false;
            sysItem8.Tooltip = "Select the CHAMBER CLOSE AFTER LOT END value.";
            sysItem8.Type = ParameterType.CheckBox;
            sysItem8.IsVisible = true;
            sysItem8.IsEditable = true;
            sysItem8.Section = "SYSTEM";
            sysItem8.Key = "CHAMBER_CLOSE_AFTER_LOT_END";
            _parameterList.Add(sysItem8);

            ParameterItem sysItem9 = new ParameterItem();
            sysItem9.Parameter = "UNLOAD SENSOR TYPE NORMAL CLOSE";
            sysItem9.Value = false;
            sysItem9.Tooltip = "Select the UNLOAD SENSOR TYPE NORMAL CLOSE value.";
            sysItem9.Type = ParameterType.CheckBox;
            sysItem9.IsVisible = true;
            sysItem9.IsEditable = true;
            sysItem9.Section = "SYSTEM";
            sysItem9.Key = "UNLOAD_SENSOR_TYPE_NORMAL_CLOSE";
            _parameterList.Add(sysItem9);

            ParameterItem sysItem10 = new ParameterItem();
            sysItem10.Parameter = "AUTOMATION";
            sysItem10.Value = 0;
            sysItem10.Tooltip = "Select the AUTOMATION value.";
            sysItem10.Type = ParameterType.ComboBox;
            sysItem10.ComboBoxItems.Clear();
            sysItem10.ComboBoxItems.Add("NONE");
            sysItem10.ComboBoxItems.Add("ATK PLS");
            sysItem10.ComboBoxItems.Add("SIGNETICS");
            sysItem10.ComboBoxItems.Add("GEM");
            sysItem10.IsVisible = true;
            sysItem10.IsEditable = true;
            sysItem10.Section = "SYSTEM";
            sysItem10.Key = "AUTOMATION";
            _parameterList.Add(sysItem10);

            ParameterItem sysItem11 = new ParameterItem();
            sysItem11.Parameter = "GAS INJECTION WHEN VACUUM START";
            sysItem11.Value = false;
            sysItem11.Tooltip = "Select the GAS INJECTION WHEN VACUUM START value.";
            sysItem11.Type = ParameterType.CheckBox;
            sysItem11.IsVisible = true;
            sysItem11.IsEditable = true;
            sysItem11.Section = "SYSTEM";
            sysItem11.Key = "GAS_INJECTION_WHEN_VACUUM_START";
            _parameterList.Add(sysItem11);

            ParameterItem sysItem12 = new ParameterItem();
            sysItem12.Parameter = "ID READING DELAY";
            sysItem12.Value = false;
            sysItem12.Tooltip = "Select the ID READING DELAY.";
            sysItem12.Type = ParameterType.CheckBox;
            sysItem12.IsVisible = true;
            sysItem12.IsEditable = true;
            sysItem12.Section = "SYSTEM";
            sysItem12.Key = "ID_READING_DELAY";
            _parameterList.Add(sysItem12);

            ParameterItem sysItem13 = new ParameterItem();
            sysItem13.Parameter = "ID READING DELAY VALUE";
            sysItem13.Value = 1000;
            sysItem13.Tooltip = "Select the ID READING DELAY value.";
            sysItem13.Unit = "msec";
            sysItem13.IsVisible = true;
            sysItem13.Section = "SYSTEM";
            sysItem13.Key = "ID_READING_DELAY_VALUE";
            _parameterList.Add(sysItem13);
        }

        #endregion FUNCTION
    }
}
