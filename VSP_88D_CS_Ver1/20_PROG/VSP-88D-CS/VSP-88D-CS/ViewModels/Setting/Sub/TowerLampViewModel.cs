using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.VSP_COMMON;
using RelayCommand = VSLibrary.Common.MVVM.Core.RelayCommand;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class TowerLampViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }
        #region PROPERTY
        private ObservableCollection<TowerLampItem> _signals;
        private ObservableCollection<string> _states;

        public ObservableCollection<string> States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }

        public ObservableCollection<TowerLampItem> Signals
        {
            get => _signals;
            set => SetProperty(ref _signals, value);
        }
        #endregion PROPERTY

        private VS_TOWER_OPTION TowerLamp = VS_TOWER_OPTION.Instance;

        public TowerLampViewModel()
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _states = new ObservableCollection<string> { "OFF", "ON", "BLINK" };

            _signals = new ObservableCollection<TowerLampItem>
            {
                new TowerLampItem ( "RUN", "OFF", "OFF", "ON", "OFF" ),
                new TowerLampItem ( "STOP", "OFF", "ON", "OFF", "OFF" ),
                new TowerLampItem ( "JOB END", "OFF", "BLINK", "OFF", "ON" ),
                new TowerLampItem ( "ERROR", "ON", "OFF", "OFF", "OFF" ),
            };

            //LoadTowerLamp();

            OkCommand = new RelayCommand(OnOK);
            CancelCommand = new RelayCommand(OnCancel);
        }

        #region COMMAND

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion COMMAND

        #region EXECUTE COMMAND

        private void OnOK()
        {
            Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        private void OnCancel()
        {
            Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(w => w.IsActive)? .Close();
        }
        private void LoadTowerLamp()
        {
            _signals = new ObservableCollection<TowerLampItem>();
            int[,] nTowerVal = TowerLamp.GetTowerLampConfig();
            for (int mode = 0; mode < (int)eTwrMode.TOWERLAMP_MODE; mode++)
            {
                string red = nTowerVal[(int)eTwrKind.TWR_RED, mode].ToString();
                string yellow = nTowerVal[(int)eTwrKind.TWR_YEL, mode].ToString();
                string green = nTowerVal[(int)eTwrKind.TWR_GRN, mode].ToString();
                string buzzer = nTowerVal[(int)eTwrKind.TWR_BUZ, mode].ToString();

                string itemName = TowerLamp.GetTwrLampMode((eTwrMode)mode); // TWR_AUTORUN...

                var item = new TowerLampItem(itemName, red, yellow, green, buzzer);
                _signals.Add(item);
            }

        }

        #endregion EXECUTE COMMAND
    }
}
