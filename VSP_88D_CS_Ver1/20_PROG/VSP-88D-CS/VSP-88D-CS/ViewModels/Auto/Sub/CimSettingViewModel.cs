using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common.CIM;
using VSP_88D_CS.Models.Auto.Sub;
using VSP_88D_CS.Sequence;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class CimSettingViewModel : ViewModelBase
    {
        private readonly CimState _cimState;
        private readonly VS_CIM_MANAGER _vsCIMManager;
        #region PROPERTY
        public ObservableCollection<CimSettingItem> SettingsList { get; set; }
        private BitmapImage _cimImg;
        public BitmapImage CimImg
        {
            get => _cimImg;
            set => SetProperty(ref _cimImg, value);
        }
        private bool _usePassive;
        public bool UsePassive
        {
            get => _usePassive;
            set => SetProperty(ref _usePassive, value);
        }
        private bool _useOffline;
        public bool UseOffline
        {
            get => _useOffline;
            set
            {
                if (SetProperty(ref _useOffline, value) && value)
                {
                    UseOnlineLocal = false;
                    UseOnlineRemote = false;
                }
            }
        }

        private bool _useOnlineLocal;
        public bool UseOnlineLocal
        {
            get => _useOnlineLocal;
            set
            {
                if (SetProperty(ref _useOnlineLocal, value) && value)
                {
                    UseOffline = false;
                    UseOnlineRemote = false;
                }
            }
        }

        private bool _useOnlineRemote;
        public bool UseOnlineRemote
        {
            get => _useOnlineRemote;
            set
            {
                if (SetProperty(ref _useOnlineRemote, value) && value)
                {
                    UseOffline = false;
                    UseOnlineLocal = false;
                }
            }
        }

        #endregion PROPERTY

        #region COMMAND
        public ICommand SaveApplyCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        void OnSaveApply()
        {

        }
        void OnStart()
        {
            _vsCIMManager.StartComm();
            //_vsCIMManager.UpdateSetFrameEnabled();
        }
        void OnStop()
        {
            _cimState.enabled = false;
            _vsCIMManager.StopComm();

            _cimState.controlState = (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE;
            //_vsCIMManager.UpdateSetFrameEnabled();
        }
        void OnClose()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
        #endregion EXECUTE COMMAND
        public CimSettingViewModel(IRegionManager regionManager =null, CimModel sharedModel = null)
        {
            _cimState = CimState.GetInstance();
            _vsCIMManager = VS_CIM_MANAGER.GetInstance();
            SettingsList = new ObservableCollection<CimSettingItem>
            {
            new CimSettingItem { Key = "Device ID", Value = "0" },
            new CimSettingItem { Key = "Host IP.", Value = "127.0.0.1" },
            new CimSettingItem { Key = "Port No.", Value = "5000" },
            new CimSettingItem { Key = "Link Test(Sec.)", Value = "10" },
            new CimSettingItem { Key = "Retry(Sec.)", Value = "3" },
            new CimSettingItem { Key = "T3", Value = "5" },
            new CimSettingItem { Key = "T5", Value = "5" },
            new CimSettingItem { Key = "T6", Value = "5" },
            new CimSettingItem { Key = "T7", Value = "5" },
            new CimSettingItem { Key = "T8", Value = "5" },
            new CimSettingItem { Key = "Conversation TimeOut", Value = "5" }
            };

            SaveApplyCommand = new RelayCommand(OnSaveApply);
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            CloseCommand = new RelayCommand(OnClose);
            //TEST
            UsePassive = true;
            UseOffline = true;
            UseOnlineLocal = false;
            UseOnlineRemote = false;
            CimImg = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/CimConnected.png"));
        }
    }
}
