using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Models.Setting;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public partial class LeakCheckViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        #region PROPERTY
        private readonly IGlobalSystemOption _globalSystemOption;

        private LeakCheckModel _data;
        public LeakCheckModel Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
        #endregion PROPERTY

        #region FUNCTION
        public LeakCheckViewModel(IGlobalSystemOption globalSystemOption)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _globalSystemOption = globalSystemOption;
            Data = new LeakCheckModel();
            //Data.TimeCount = 10;
            //Data.OverPumpingTime = 30;
            //Data.StableTime = 10;
            //Data.LeakCheckTime = 60;
            //Data.LeakAlarmRate = 0.2;

            //Data.LeakCheckStartTime = DateTime.MinValue;
            //Data.LeakCheckEndTime = DateTime.MaxValue;

            //Data.StartPressure = 10;
            //Data.EndPressure = 20;
            //Data.LeakRate = 1.2;

            FilePath = Path.Combine(globalSystemOption.DataPath, "LEAK_CHECK.INI");
            LoadLeakCheck();
            CloseCommand = new RelayCommand(OnClose);
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void SaveLeakCheck()
        {
            if (null == _data) 
                return;
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                writer.WriteLine("[LEAK TEST]");
                writer.WriteLine($"OVER PUMP TIME={Data.OverPumpingTime}");
                writer.WriteLine($"STABLE TIME={Data.StableTime}");
                writer.WriteLine($"LEAK CHECK TIME={Data.LeakCheckTime}");
                writer.WriteLine($"LEAK ALARM RATE={Data.LeakAlarmRate}");
                writer.Close();
            }    
        }

        private void LoadLeakCheck()
        {
            if (null  == _data) 
                return;
            if(!File.Exists(FilePath))
            {
                LoadDefault();
                return;
            }
            using (StreamReader reader = new StreamReader(FilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                { 
                    line = line.Trim();
                    
                    if (line.Contains('='))
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        switch (key.ToUpper())
                        {
                            case "OVER PUMP TIME":
                                Data.OverPumpingTime = int.Parse(value);
                                break;
                            case "STABLE TIME":
                                Data.StableTime = int.Parse(value);
                                break;
                            case "LEAK CHECK TIME":
                                Data.LeakCheckTime = int.Parse(value);
                                break;
                            case "LEAK ALARM RATE":
                                Data.LeakAlarmRate = double.Parse(value);
                                break;
                        }
                    }
                }
            }

        }

        private void LoadDefault()
        {
            Data.OverPumpingTime = 30;
            Data.StableTime = 10;
            Data.LeakCheckTime = 60;
            Data.LeakAlarmRate = 0.2;
        }

        #endregion FUNCTION

        #region COMMAND
        public ICommand CloseCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        private void OnClose()
        {
            Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }

        private void OnOk()
        {
            SaveLeakCheck();
        }
        private void OnCancel()
        {
            if(null == _data)
                return; 
            LoadLeakCheck();
        }
        #endregion EXECUTE COMMAND
    }
}
