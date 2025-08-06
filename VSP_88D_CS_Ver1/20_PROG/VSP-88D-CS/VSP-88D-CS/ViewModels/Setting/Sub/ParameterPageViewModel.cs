using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.Views.Setting.Sub;


namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public partial class ParameterPageViewModel : ViewModelBase
    {
        #region PROPERY
        public LanguageService LanguageResources { get; }

        private ObservableCollection<ParameterItem> _parameterItems = new ObservableCollection<ParameterItem>();
        public ObservableCollection<ParameterItem> ParameterItems
        {
            get => _parameterItems;
            set => SetProperty(ref _parameterItems, value);
        }

        private IGlobalSystemOption _globalSystemOption;
        //private ObservableCollection<ParameterItem> _plasmaItems = new ObservableCollection<ParameterItem>();
        //private ObservableCollection<ParameterItem> _systemItems = new ObservableCollection<ParameterItem>();

        private ObservableCollection<ParameterItem> _plasmaItems;
        private ObservableCollection<ParameterItem> _systemItems;
        private TowerLamp _towerLamp;
        private LeakCheck _leakCheck;

        public ObservableCollection<ParameterItem> PlasmaItems
        {
            get => _plasmaItems;
            set => SetProperty(ref _plasmaItems, value);
        }
        public ObservableCollection<ParameterItem> SystemItems
        {
            get => _systemItems;    
            set => SetProperty(ref _systemItems, value);
        }

        private string _gridValueTitle = "Value";
        public string GridValueTitle
        {
            get => _gridValueTitle;
            set => SetProperty(ref _gridValueTitle, value);
        }

        private string _gridParameterTitle = "Parameter";
        public string GridParameterTitle
        {
            get => _gridParameterTitle;
            set => SetProperty(ref _gridParameterTitle, value);
        }

        private string _gridUnitTitle;
        public string GridUnitTitle
        {
            get => _gridUnitTitle;
            set => SetProperty(ref _gridParameterTitle, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
        #endregion PROPERTY

        #region COMMAND
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand TowerLampCommand { get; }
        public ICommand LeakCheckCommand { get; }

        #endregion COMMAND

        #region EXECUTE COMMAND
        private void ShowTowerLamp()
        {
            VSContainer.Instance.ClearCache(typeof(TowerLamp));
            VSContainer.Instance.ClearCache(typeof(TowerLampViewModel));
            if (VSContainer.Instance.Resolve(typeof(TowerLamp)) is TowerLamp mainView)
            {
                mainView.ShowDialog();
            }
        }

        private void ShowLeakCheck()
        {
            if (VSContainer.Instance.Resolve(typeof(LeakCheck)) is LeakCheck mainView)
            {
                mainView.ShowDialog();
            }
        }

        private void OnTowerLamp()
        {
            //TowerLamp towerLamp = new TowerLamp();
            ShowTowerLamp();
            //_towerLamp.ShowDialog();
        }

        private void OnLeakCheck()
        {
            //LeakCheck check = new LeakCheck();
            //_leakCheck.ShowDialog();
            ShowLeakCheck();
        }
        #endregion EXECUTE COMMAND

        #region FUNCTION
        public ParameterPageViewModel()
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();
            _globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _plasmaItems = _globalSystemOption.PlasmaItems.ParameterList;
            _systemItems = _globalSystemOption.SystemItems.ParameterList;
            _gridParameterTitle = "Parameter";
            _gridValueTitle = "Value";
            _gridUnitTitle = "Unit";

            FilePath = Path.Combine(_globalSystemOption.DataPath, "OPTION.JSON");

            SaveCommand = new RelayCommand(SaveParameter);
            CancelCommand = new RelayCommand(LoadParameter);
            TowerLampCommand = new RelayCommand(OnTowerLamp);
            LeakCheckCommand = new RelayCommand(OnLeakCheck);
        }
        private void SaveParameter()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                // Write plasma data
                //writer.WriteLine($"[PLASMA]");
                foreach (var item in PlasmaItems)
                {
                    string json = JsonSerializer.Serialize(item);
                    //writer.WriteLine($"{item.Key}={json}");
                    writer.WriteLine($"{json}");
                    writer.WriteLine();
                }

                // Write system data
                //writer.WriteLine($"[SYSTEM]");
                foreach (var item in SystemItems)
                {
                    string json = JsonSerializer.Serialize(item);
                    //writer.WriteLine($"{item.Key}={json}");
                    writer.WriteLine($"{json}");
                    //writer.WriteLine(json);
                    writer.WriteLine();
                }
                writer.Close();
            }
            //CHANGE LANGUAGE
            int aaa = _globalSystemOption.SystemItems.GetLanguage();
            LanguageResources.CurrentLanguage = (EnumLanguage)aaa;
        }

        private void LoadParameter()
        {
            _globalSystemOption.LoadPlasmaAndSystemParameter();
            //PlasmaItems.Clear();
            //SystemItems.Clear();

            if (!File.Exists(FilePath))
            {
                //throw new FileNotFoundException("INI file does not exist.");
                Default();
                return;
            }

            //using (StreamReader reader = new StreamReader(FilePath))
            //{
            //    string line;
            //    ObservableCollection<ParameterItem> currentCollection = null;

            //    while ((line = reader.ReadLine()) != null)
            //    {
            //        line = line.Trim();
            //        if (string.IsNullOrWhiteSpace(line)) 
            //            continue;

            //        if (line.Equals("[PLASMA]"))
            //        {
            //            currentCollection = PlasmaItems;
            //        }
            //        else if (line.Equals("[SYSTEM]"))
            //        {
            //            currentCollection = SystemItems;
            //        }
            //        else if (currentCollection != null)
            //        {
            //            // Key-value analysis
            //            var parts = line.Split(new[] { '=' }, 2);
            //            if (parts.Length == 2)
            //            {
            //                string jsonValue = parts[1];
            //                var parameterItem = JsonSerializer.Deserialize<ParameterItem>(jsonValue);
            //                if (parameterItem != null)
            //                {
            //                    currentCollection.Add(parameterItem);
            //                }
            //            }
            //        }
            //    }
            //    reader.Close();
            //if (0 == PlasmaItems.Count || 0 == SystemItems.Count)
            //    Default();
            //}
        }

        public void Default()
        {
            ParameterItem parameterItem1 = new ParameterItem();
            parameterItem1.Parameter = "RF REFLECT VALUE";
            parameterItem1.Value = 100;
            parameterItem1.Tooltip = "Input the RF REFLECT Value";
            parameterItem1.Unit = "watt";
            parameterItem1.IsVisible = true;
            _plasmaItems.Add(parameterItem1);

            ParameterItem parameterItem2 = new ParameterItem();
            parameterItem2.Parameter = "RF REFLECT ERROR TIME (2~10)";
            parameterItem2.Value = 5;
            parameterItem2.Tooltip = "Input the RF REFLECT ERROR TIME (2~10) Value";
            parameterItem2.Unit = "Sec.";
            parameterItem2.IsVisible = true;
            _plasmaItems.Add(parameterItem2);

            ParameterItem parameterItem3 = new ParameterItem();
            parameterItem3.Parameter = "MFC ERROR VALUE";
            parameterItem3.Value = 10;
            parameterItem3.Tooltip = "Input the MFC ERROR value.";
            parameterItem3.Unit = "sccm";
            parameterItem3.IsVisible = true;
            _plasmaItems.Add(parameterItem3);

            ParameterItem parameterItem4 = new ParameterItem();
            parameterItem4.Parameter = "MFC ERROR TIME";
            parameterItem4.Value = 15;
            parameterItem4.Tooltip = "Input the MFC ERROR TIME value.";
            parameterItem4.Unit = "sec.";
            parameterItem4.IsVisible = true;
            _plasmaItems.Add(parameterItem4);

            ParameterItem parameterItem5 = new ParameterItem();
            parameterItem5.Parameter = "VENTILATION TIME (1000~30000)";
            parameterItem5.Value = 1000;
            parameterItem5.Tooltip = "Input the RF VENTILATION TIME value.";
            parameterItem5.Unit = "sec.";
            parameterItem5.IsVisible = true;
            _plasmaItems.Add(parameterItem5);

            ParameterItem parameterItem6 = new ParameterItem();
            parameterItem6.Parameter = "VACUUM ERROR TIME";
            parameterItem6.Value = 1000;
            parameterItem6.Tooltip = "Input the VACUUM ERROR TIME value.";
            parameterItem6.Unit = "sec.";
            parameterItem6.IsVisible = true;
            _plasmaItems.Add(parameterItem6);

            ParameterItem parameterItem7 = new ParameterItem();
            parameterItem7.Parameter = "MANUAL RF ON";
            parameterItem7.Value = true;
            parameterItem7.Type = ParameterType.CheckBox;
            parameterItem7.CheckedText = "Yes";
            parameterItem7.UncheckedText = "No";
            parameterItem7.Tooltip = "Select how to manual RF on.";
            parameterItem7.IsVisible = true;
            parameterItem7.IsEditable = true;
            _plasmaItems.Add(parameterItem7);

            /////////////////////////////////////////////////////////////
            ParameterItem sysItem1 = new ParameterItem();
            sysItem1.Parameter = "BUZZER OFF TIME (3~60)";
            sysItem1.Value = 3;
            sysItem1.Unit = "sec.";
            sysItem1.Tooltip = "Input the BUZZER OFF TIME (3~60) value.";
            sysItem1.IsVisible = true;
            _systemItems.Add(sysItem1);

            ParameterItem sysItem2 = new ParameterItem();
            sysItem2.Parameter = "OUT STRIP EMPTY CHECK TIME";
            sysItem2.Value = 500;
            sysItem2.Unit = "msec.";
            sysItem2.Tooltip = "Input the OUT STRIP EMPTY CHECK TIME value.";
            sysItem2.IsVisible = true;
            _systemItems.Add(sysItem2);

            ParameterItem sysItem3 = new ParameterItem();
            sysItem3.Parameter = "SERVO POSITION CHANGE ENABLE";
            sysItem3.Value = false;
            sysItem3.Tooltip = "Input the SERVO POSITION CHANGE ENABLE value.";
            sysItem3.Type = ParameterType.CheckBox;
            sysItem3.IsVisible = true;
            sysItem3.IsEditable = true;
            _systemItems.Add(sysItem3);

            ParameterItem sysItem4 = new ParameterItem();
            sysItem4.Parameter = "INDEX PUSHER UP AFTER LOADING";
            sysItem4.Value = false;
            sysItem4.Tooltip = "Input the INDEX PUSHER UP AFTER LOADING value.";
            sysItem4.Type = ParameterType.CheckBox;
            sysItem4.IsVisible = true;
            sysItem4.IsEditable = true;
            _systemItems.Add(sysItem4);

            ParameterItem sysItem5 = new ParameterItem();
            sysItem5.Parameter = "LANGUAGE";
            sysItem5.Value = 1;
            sysItem5.Tooltip = "Select the LANGUAGE value.";
            sysItem5.Type = ParameterType.ComboBox;
            sysItem5.ComboBoxItems.Clear();
            sysItem5.ComboBoxItems.Add("KOREAN[K]");
            sysItem5.ComboBoxItems.Add("ENGLISH[E]");
            sysItem5.ComboBoxItems.Add("CHINESE[C]");
            sysItem5.ComboBoxItems.Add("VIETNAMESE[V]");
            sysItem5.IsVisible = true;
            sysItem5.IsEditable = true;
            _systemItems.Add(sysItem5);

            ParameterItem sysItem6 = new ParameterItem();
            sysItem6.Parameter = "DOOR SENSOR SKIP DURING MANUAL";
            sysItem6.Value = false;
            sysItem6.Tooltip = "Select the DOOR SENSOR SKIP DURING MANUAL value.";
            sysItem6.Type = ParameterType.CheckBox;
            sysItem6.IsVisible = true;
            sysItem6.IsEditable = true;
            _systemItems.Add(sysItem6);

            ParameterItem sysItem7 = new ParameterItem();
            sysItem7.Parameter = "UNLOADING BUFFER KEEP RIGHT";
            sysItem7.Value = false;
            sysItem7.Tooltip = "Select the UNLOADING BUFFER KEEP RIGHT value.";
            sysItem7.Type = ParameterType.CheckBox;
            sysItem7.IsVisible = true;
            sysItem7.IsEditable = true;
            _systemItems.Add(sysItem7);

            ParameterItem sysItem8 = new ParameterItem();
            sysItem8.Parameter = "CHAMBER CLOSE AFTER LOT END";
            sysItem8.Value = false;
            sysItem8.Tooltip = "Select the CHAMBER CLOSE AFTER LOT END value.";
            sysItem8.Type = ParameterType.CheckBox;
            sysItem8.IsVisible = true;
            sysItem8.IsEditable = true;
            _systemItems.Add(sysItem8);

            ParameterItem sysItem9 = new ParameterItem();
            sysItem9.Parameter = "UNLOAD SENSOR TYPE NORMAL CLOSE";
            sysItem9.Value = false;
            sysItem9.Tooltip = "Select the UNLOAD SENSOR TYPE NORMAL CLOSE value.";
            sysItem9.Type = ParameterType.CheckBox;
            sysItem9.IsVisible = true;
            sysItem9.IsEditable = true;
            _systemItems.Add(sysItem9);

            ParameterItem sysItem10 = new ParameterItem();
            sysItem10.Parameter = "AUTOMATION";
            sysItem10.Value = 0;
            sysItem10.Tooltip = "Select the AUTOMATION value.";
            sysItem10.Type = ParameterType.ComboBox;
            sysItem10.ComboBoxItems.Clear();
            sysItem10.ComboBoxItems.Add("NONE");
            sysItem10.ComboBoxItems.Add("ATK PLS");
            sysItem10.ComboBoxItems.Add("SIGNETICS");
            sysItem5.ComboBoxItems.Add("GEM");
            sysItem10.IsVisible = true;
            sysItem10.IsEditable = true;
            _systemItems.Add(sysItem10);

            ParameterItem sysItem11 = new ParameterItem();
            sysItem11.Parameter = "GAS INJECTION WHEN VACUUM START";
            sysItem11.Value = false;
            sysItem11.Tooltip = "Select the GAS INJECTION WHEN VACUUM START value.";
            sysItem11.Type = ParameterType.CheckBox;
            sysItem11.IsVisible = true;
            sysItem11.IsEditable = true;
            _systemItems.Add(sysItem11);

            ParameterItem sysItem12 = new ParameterItem();
            sysItem12.Parameter = "ID READING DELAY";
            sysItem12.Value = false;
            sysItem12.Tooltip = "Select the ID READING DELAY.";
            sysItem12.Type = ParameterType.CheckBox;
            sysItem12.IsVisible = true;
            sysItem12.IsEditable = true;
            _systemItems.Add(sysItem12);

            ParameterItem sysItem13 = new ParameterItem();
            sysItem13.Parameter = "ID READING DELAY VALUE";
            sysItem13.Value = 1000;
            sysItem13.Tooltip = "Select the ID READING DELAY value.";
            sysItem13.Unit = "msec";
            sysItem13.IsVisible = true;
            _systemItems.Add(sysItem13);
        }
        #endregion FUNCTION
    }
}
