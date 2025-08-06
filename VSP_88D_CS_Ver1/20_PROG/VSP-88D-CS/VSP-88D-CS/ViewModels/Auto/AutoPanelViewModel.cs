using ChamberControl.Views;
using System.Windows;
using System.Windows.Input;
using UserAccessLib.Common.Interfaces;

using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Auth;
using VSP_88D_CS.Views.Auto.Sub;

namespace VSP_88D_CS.ViewModels.Auto
{
    public class AutoPanelViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IAuthService _authService;
        private readonly IGlobalSystemOption _globalSystemOption;
        #region PROPERTY
        public LanguageService LanguageResources { get;}
        private string _generatorRun;
        public string GeneratorRun
        {
            get => _generatorRun;
            set => SetProperty(ref _generatorRun, value);
        }

        private string _generatorMode;
        public string GeneratorMode
        {
            get => _generatorMode;
            set => SetProperty(ref _generatorMode, value);
        }

        private string _pumpRun;
        public string PumpRun
        {
            get => _pumpRun;
            set => SetProperty(ref _pumpRun, value);
        }

        private string _purgeTime;
        public string PurgeTime
        {
            get => _purgeTime;
            set => SetProperty(ref _purgeTime, value);
        }
        public string UseIonizerDisplay => UseIonizer ? "ON" : "OFF";
        private bool _useIonizer;
        public bool UseIonizer
        {
            get => _useIonizer;
            set
            {
                if (_useIonizer != value)
                {
                    _useIonizer = value;
                    OnPropertyChanged(nameof(UseIonizer));
                    OnPropertyChanged(nameof(UseIonizerDisplay));
                }
            }
        }
        #endregion PROPERTY

        #region COMMAND
        public ICommand OpenChamberCommand { get; set; }
        public ICommand CloseChamberCommand { get; set; }
        public ICommand CleanChamberCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        private void OnOpenChamber()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.LidOpenFunction, _globalSystemOption))
            {
                return;
            }
            MessageBox.Show("TEST OPEN");
        }
        private void OnCloseChamber()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.LidCloseFunction, _globalSystemOption))
            {
                return;
            }
            MessageBox.Show("TEST CLOSE");
        }
        private void OnCleanChamber()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.ManualCleanFunction, _globalSystemOption))
            {
                return;
            }
            MessageBox.Show("TEST CLEAN");
        }
        #endregion EXECUTE COMMAND

        public AutoPanelViewModel(IRegionManager regionManager, IGlobalSystemOption globalSystemOption,IAuthService authService)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _regionManager = regionManager;

            _regionManager.RequestNavigate<Production>("ProductionView");
            _regionManager.RequestNavigate<State>("StateView");
            _regionManager.RequestNavigate<Chamber>("ChamberView");
            _regionManager.RequestNavigate<SvPv>("SvPvView");
            _regionManager.RequestNavigate<SelectJob>("SelectJobView");
            //_regionManager.RequestNavigate<Cim>("CimView");

            OpenChamberCommand = new RelayCommand(OnOpenChamber);
            CloseChamberCommand = new RelayCommand(OnCloseChamber);
            CleanChamberCommand = new RelayCommand(OnCleanChamber);
            //TEST option + value
            UseIonizer = false;
            GeneratorMode = "Local";
            GeneratorRun = "0:00";
            PumpRun = "0:00";
            PurgeTime = "0/1350";
            _globalSystemOption = globalSystemOption;
            _authService = authService;
        }
    }
}
