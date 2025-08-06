using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.CIM;
using VSP_88D_CS.Models.Auto.Sub;
using VSP_88D_CS.Sequence;
using VSP_88D_CS.Views.Auto.Sub;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class CimViewModel : ViewModelBase
    {
        private CimSetting _cimSetting = new CimSetting();
        private readonly CimState _cimState;
        private readonly VS_CIM_MANAGER _vsCIMManager;
        #region PROPERTY
        public CimModel Offline { get; } = new();
        public CimModel Local { get; } = new();
        public CimModel Remote { get; } = new();
        public LanguageService LanguageResources { get; }
        #endregion PROPERTY

        #region COMMAND
        public ICommand OfflineCommand { get; set; }
        public ICommand LocalCommand { get; set; }
        public ICommand RemoteCommand { get; set; }
        public ICommand ShowCimSettingCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        void OnOffline()
        {
            //TODO: check login level
            if (_vsCIMManager.IsWaitForReplyCT())
                return;
            if (!_cimState.connectedFg)
                return;

            _vsCIMManager.StartCtrlStateChange((int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE);
            //LOG_STR(m_strLogHead, L"Offline click");

            //TEST
            OnShowCimSettingCommand();
        }
        void OnLocal()
        {
            //TODO: check login level
            if (_vsCIMManager.IsWaitForReplyCT())
                return;
            if (!_cimState.connectedFg)
                return;

            _vsCIMManager.StartCtrlStateChange((int)CONTROL_STATE.CONTROL_ONLINE_LOCAL);
            //LOG_STR(m_strLogHead, L"Online Local click");
        }
        void OnRemote()
        {
            //TODO: check login level
            if (_vsCIMManager.IsWaitForReplyCT())
                return;
            if (!_cimState.connectedFg)
                return;

            _vsCIMManager.StartCtrlStateChange((int)CONTROL_STATE.CONTROL_ONLINE_REMOTE);
            //LOG_STR(m_strLogHead, L"Online Remote click");
        }
        void OnShowCimSettingCommand()
        {
            if (_cimSetting == null || !_cimSetting.IsLoaded)
            {
                _cimSetting = new CimSetting();
                _cimSetting.DataContext = new CimSettingViewModel();
                _cimSetting.ShowDialog();
            }
            else if (!_cimSetting.IsVisible)
            {
                _cimSetting.ShowDialog();
            }
            else
            {
                _cimSetting.Activate();
            }
        }
        #endregion EXECUTE COMMAND
        public CimViewModel(CimSetting View = null)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            OfflineCommand = new RelayCommand(OnOffline);
            LocalCommand = new RelayCommand(OnLocal);
            RemoteCommand = new RelayCommand(OnRemote);
            ShowCimSettingCommand = new RelayCommand(OnShowCimSettingCommand);

            _cimSetting = View;
            _cimState = CimState.GetInstance();
            _vsCIMManager = VS_CIM_MANAGER.GetInstance();
            _vsCIMManager.StatusChanged += OnStatusChanged;
        }
        private void OnStatusChanged(int status)
        {
            switch(status)
            {
                case 0: break;
                case 1: break;  
                case 2: break;
            }    
        }
    }
}
