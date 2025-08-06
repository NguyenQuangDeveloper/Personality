using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting
{
    public class PlasmaOption : ViewModelBase
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
        public PlasmaOption()
        {
            _parameterList = new ObservableCollection<ParameterItem>();
        }

        public int GetRfReflectValue()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "RF_REFLECT_VALUE");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public int GetRfReflectErrorTime()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "RF_REFLECT_ERROR_TIME");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public int GetMfcErrorValue()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "MFC_ERROR_VALUE");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public int GetMfcErrorTime()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "MFC_ERROR_TIME");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public int GetVentilationTime()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "VENTILATION_TIME");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public int GetVacuumErrorTime()
        {
            int value = 0;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "VACUUM_ERROR_TIME");
            value = Convert.ToInt32(item.Value);
            return value;
        }

        public bool GetManualRfOn()
        {
            bool value = false;
            ParameterItem item = _parameterList.FirstOrDefault(item => item.Section == "PLASMA" && item.Key == "MANUAL_RF_ON");
            value = Convert.ToBoolean(item.Value);
            return value;
        }

        public void Default()
        {
            _parameterList.Clear();
            ParameterItem parameterItem1 = new ParameterItem();
            parameterItem1.Parameter = "RF REFLECT VALUE";
            parameterItem1.Value = 100;
            parameterItem1.Tooltip = "Input the RF REFLECT Value";
            parameterItem1.Unit = "watt";
            parameterItem1.IsVisible = true;
            parameterItem1.Key = "RF_REFLECT_VALUE";
            parameterItem1.Section = "PLASMA";
            _parameterList.Add(parameterItem1);

            ParameterItem parameterItem2 = new ParameterItem();
            parameterItem2.Parameter = "RF REFLECT ERROR TIME (2~10)";
            parameterItem2.Value = 5;
            parameterItem2.Tooltip = "Input the RF REFLECT ERROR TIME (2~10) Value";
            parameterItem2.Unit = "Sec.";
            parameterItem2.IsVisible = true;
            parameterItem2.Key = "RF_REFLECT_ERROR_TIME";
            parameterItem2.Section = "PLASMA";
            _parameterList.Add(parameterItem2);

            ParameterItem parameterItem3 = new ParameterItem();
            parameterItem3.Parameter = "MFC ERROR VALUE";
            parameterItem3.Value = 10;
            parameterItem3.Tooltip = "Input the MFC ERROR value.";
            parameterItem3.Unit = "sccm";
            parameterItem3.IsVisible = true;
            parameterItem3.Section = "PLASMA";
            parameterItem3.Key = "MFC_ERROR_VALUE";
            _parameterList.Add(parameterItem3);

            ParameterItem parameterItem4 = new ParameterItem();
            parameterItem4.Parameter = "MFC ERROR TIME";
            parameterItem4.Value = 15;
            parameterItem4.Tooltip = "Input the MFC ERROR TIME value.";
            parameterItem4.Unit = "sec.";
            parameterItem4.IsVisible = true;
            parameterItem4.Section = "PLASMA";
            parameterItem4.Key = "MFC_ERROR_TIME";
            _parameterList.Add(parameterItem4);

            ParameterItem parameterItem5 = new ParameterItem();
            parameterItem5.Parameter = "VENTILATION TIME (1000~30000)";
            parameterItem5.Value = 1000;
            parameterItem5.Tooltip = "Input the RF VENTILATION TIME value.";
            parameterItem5.Unit = "sec.";
            parameterItem5.IsVisible = true;
            parameterItem5.Section = "PLASMA";
            parameterItem5.Key = "VENTILATION_TIME";
            _parameterList.Add(parameterItem5);

            ParameterItem parameterItem6 = new ParameterItem();
            parameterItem6.Parameter = "VACUUM ERROR TIME";
            parameterItem6.Value = 1000;
            parameterItem6.Tooltip = "Input the VACUUM ERROR TIME value.";
            parameterItem6.Unit = "sec.";
            parameterItem6.IsVisible = true;
            parameterItem6.Section = "PLASMA";
            parameterItem6.Key = "VACUUM_ERROR_TIME";
            _parameterList.Add(parameterItem6);

            ParameterItem parameterItem7 = new ParameterItem();
            parameterItem7.Parameter = "MANUAL RF ON";
            parameterItem7.Value = true;
            parameterItem7.Type = ParameterType.CheckBox;
            parameterItem7.CheckedText = "Yes";
            parameterItem7.UncheckedText = "No";
            parameterItem7.Tooltip = "Select how to manual RF on.";
            parameterItem7.IsVisible = true;
            parameterItem7.IsEditable = true;
            parameterItem7.Section = "PLASMA";
            parameterItem7.Key = "MANUAL_RF_ON";
            _parameterList.Add(parameterItem7);
        }
        #endregion FUNCTION
    }
}
