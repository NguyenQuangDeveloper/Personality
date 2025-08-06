using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UserAccessLib.Common.Enum;
using UserAccessLib.Common.Interfaces;
using UserAccessLib.Models;
using UserAccessLib.ViewModels;
using UserAccessLib.Views;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.LayoutPanels.TopPanel;
using VSLibrary.UIComponent.MessageBox;
using VSLibrary.UIComponent.VsNavigations;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Auth;
using VSP_88D_CS.ViewModels.Auto.Sub;
using VSP_88D_CS.Views.Auto;
using VSP_88D_CS.Views.Manual;
using VSP_88D_CS.Views.Popup;
using VSP_88D_CS.Views.Report;
using VSP_88D_CS.Views.Setting;
using VSP_88D_CS.Common.CIM;
using VSP_88D_CS.Sequence;
using EZGemPlusCS;

namespace VSP_88D_CS.Shared
{
    public static class SharedInstances
    {
        public static CimViewModel SharedCimViewModel { get; } = new CimViewModel();
    }
}

namespace VSP_88D_CS.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }
        private readonly IAuthService _authService;
        private readonly IGlobalSystemOption _globalSystemOption;
        private readonly IRegionManager _regionManager;
        private readonly DispatcherTimer _timer;
        private readonly VS_CIM_MANAGER _vsCIMManager;
        private VSIOWindow _vsIOWindow;

        #region PROPERTY
        private string _equipmentName;
        public string EquipmentName
        {
            get => _equipmentName;
            set => SetProperty(ref _equipmentName, value);
        }
        private string _viewName;
        public string ViewName
        {
            get => _viewName;
            set => SetProperty(ref _viewName, value);
        }
        private string _equipmentState;
        public string EquipmentState
        {
            get => _equipmentState;
            set => SetProperty(ref _equipmentState, value);
        }
        private string _dateText;
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }
        private string _timeText;
        public string TimeText
        {
            get => _timeText;
            set => SetProperty(ref _timeText, value);
        }

        private bool _controlsEnabled;
        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set => SetProperty(ref _controlsEnabled, value);
        }

        public ButtonData LoginButton => ButtonDatas.FirstOrDefault()!;

        private ObservableCollection<ButtonData> _buttonDatas;
        public ObservableCollection<ButtonData> ButtonDatas
        {
            get => _buttonDatas;
            set => SetProperty(ref _buttonDatas, value);
        }

        private string _currentView;
        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private int _autoLogoutTimeout = 0;
        public int AutoLogoutTimeout
        {
            get => _autoLogoutTimeout;
            set => SetProperty(ref _autoLogoutTimeout, value);
        }

        private bool _onceLogin = true;
        public bool OnceLogin
        {
            get => _onceLogin;
            set => SetProperty(ref _onceLogin, value);
        }

        #endregion PROPERTY

        #region COMMAND
        public ICommand OpenSequenceMonitorCommand { get; set; }
        public ICommand MainShowCommand { get; set; }
        public ICommand ManualShowCommand { get; set; }
        public ICommand SettingShowCommand { get; set; }
        public ICommand ReportShowCommand { get; set; }
        public ICommand ExitShowCommand { get; set; }
        public ICommand MainClosingCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand OpenViewCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        void OpenSequenceMonitor()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.SequenceMonitor, _globalSystemOption))
            {
                return;
            }
            //TODO: show sequence monitor
        }
        void MainShow()
        {
            _regionManager.RequestNavigate<AutoPanel>("MainView");
            CurrentView = "Main";
        }
        void ManualShow()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.ManualFunction, _globalSystemOption))
            {
                SetPrevButtonFocus();
                return;
            }
            _regionManager.RequestNavigate<ManualPanel>("MainView");
            CurrentView = "Manual";
        }

        void SettingShow()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.SettingFunction, _globalSystemOption))
            {
                SetPrevButtonFocus();
                return;
            }
            _regionManager.RequestNavigate<SettingPanel>("MainView");
            CurrentView = "Setting";
        }

        void ReportShow()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.ReportFunction, _globalSystemOption))
            {
                SetPrevButtonFocus();
                return;
            }

            _regionManager.RequestNavigate<ReportPanel>("MainView");
            CurrentView = "Report";
        }
        void ExitShow()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.ExitFunction, _globalSystemOption))
            {
                SetPrevButtonFocus();
                return;
            }

            VsMessageBox.ShowAsync("Do you want to exit the program?", "Shutdown Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, result =>
            {
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            });
        }

        #endregion EXECUTE COMMAND

        public MainViewModel(IRegionManager regionManager, IAuthService authService, VSIOWindow vsIOWindow, IGlobalSystemOption globalSystemOption)
        {
            _regionManager = regionManager;
            LanguageResources = LanguageService.GetInstance();
            _authService = authService;
            _vsIOWindow = vsIOWindow;
            EquipmentStateService.Instance.EquipmentStatus = "Stop";

            RegisterCommands();
            VsNavigationBar.AutoLogoutOccurred += (sender, e) =>
            {
                ButtonDatas = CreateNavigationButtons(OnceLogin);
                CurrentView = "Main";
            };

            ButtonDatas = CreateNavigationButtons(OnceLogin);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += OnTimerTick!;
            _timer.Start();
            UpdateDateTime();

            MainShow();
            _globalSystemOption = globalSystemOption;
            //LoginVisible = "Visible";
            //LogoutVisible = "Collapsed";
            _vsCIMManager = VS_CIM_MANAGER.GetInstance();
            _vsCIMManager.StartComm();
        }

        private void RegisterCommands()
        {
            OpenSequenceMonitorCommand = new RelayCommand(OpenSequenceMonitor);
            MainShowCommand = new RelayCommand(MainShow);
            ManualShowCommand = new RelayCommand(ManualShow);
            SettingShowCommand = new RelayCommand(SettingShow);
            ReportShowCommand = new RelayCommand(ReportShow);
            ExitShowCommand = new RelayCommand(ExitShow);
            MainClosingCommand = new RelayCommand(OnMainClosing);
            LoginCommand = new RelayCommand(OnLogin);
            LogoutCommand = new RelayCommand(OnLogout);
            OpenViewCommand = new RelayCommand<string>(OpenView);
        }

        private void SetPrevButtonFocus()
        {
            foreach (var item in ButtonDatas)
            {
                item.IsSelected = false;
            }

            var prevButton = ButtonDatas.First(x => x.Content == CurrentView);
            prevButton.IsSelected = true;
        }

        /// <summary>
        /// INI 설정값에 따라 Navigation 버튼 리스트 생성
        /// </summary>
        /// <param name="onceLogin">"1"이면 로그인 버튼 포함, 아니면 제외</param>
        /// <returns>초기화된 ButtonData 컬렉션</returns>
        private ObservableCollection<ButtonData> CreateNavigationButtons(bool showLogin)
        {
            var buttons = new ObservableCollection<ButtonData>();
            int spareCount = showLogin ? 4 : 5;

            //if (showLogin)
            buttons.Add(VsNavigationHelper.Create("Login", LoginCommand, 0, "/Resources/Icons/Login.png", false, false));
            buttons.Add(VsNavigationHelper.Create("Main", MainShowCommand, 0, "/Resources/Icons/Main.png", true));
            buttons.Add(VsNavigationHelper.Create("Manual", ManualShowCommand, 0, "/Resources/Icons/Manual.png"));
            buttons.Add(VsNavigationHelper.Create("Setting", SettingShowCommand, 0, "/Resources/Icons/Setting.png"));
            buttons.Add(VsNavigationHelper.Create("Report", ReportShowCommand, 0, "/Resources/Icons/Report.png"));

            for (int i = 0; i < spareCount; i++)
                buttons.Add(VsNavigationHelper.Create("Spare", null, 0, null, false, true, Visibility.Hidden));

            buttons.Add(VsNavigationHelper.Create("Exit", ExitShowCommand, 0, "/Resources/Icons/Exit.png", false, false));

            // 마지막 버튼 Margin 제거
            if (buttons.Count > 0)
                buttons[buttons.Count - 1].Margin = new Thickness(0);

            return buttons;
        }

        /// <summary>
        /// 뷰가 활성화될 때 호출됩니다.
        /// </summary>
        public override void Activate()
        {
            CurrentView = "Main";
        }

        private void OnLogout()
        {
            AuthFunctions.CurrentUser = null;
            _authService.Logout();
            ChangeLogin();
        }

        private void OpenView(string key)
        {
            if (key == "F2")
            {
                if (_vsIOWindow == null || !_vsIOWindow.IsVisible)
                {
                    //_IOPanel = new IOPanel();
                    // 창이 열려 있지 않으면 새로 생성하고 열기               
                    _vsIOWindow.Show();
                }
                else
                {
                    // 창이 이미 열려 있을 경우 포커스 설정
                    _vsIOWindow.Focus();
                }
            }
        }
        private void OnLogin()
        {
            if (AuthFunctions.CurrentUser == null)
            {
                ShowLogin();
            }
            else
            {
                OnLogout();
            }
        }
        private void OnMainClosing()
        {
            Close();
        }
        private void Close()
        {
            _timer.Stop();
            Application.Current.Shutdown();

        }
        private void UpdateDateTime()
        {
            DateText = DateTime.Now.ToString("yyyy-MM-dd");
            TimeText = DateTime.Now.ToString("HH:mm:ss");
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }
        private void ShowLogin()
        {
            var loginVM = new LoginViewModel(_authService);
            loginVM.LoginSuccess += OnLoginSuccess;
            var loginView = new LoginWindow
            {
                DataContext = loginVM
            };

            var dr = loginView.ShowDialog();
            if (dr == true)
            {
                //ChangeLogin();
            }
        }
        private void OnLoginSuccess(LoginInfo loginInfo)
        {
            if (loginInfo.RememberMe)
            {
                AuthFunctions.CurrentUser = loginInfo;
                ChangeLogin();
            }
            else
            {
                AuthFunctions.CurrentUser = null;
            }
            string strMsg = loginInfo.UserRole.ToString();
            MessageBox.Show($"Role {strMsg}");

        }
        private void ChangeLogin()
        {
            if (LoginButton.Content == "Login")
            {
                LoginButton.Content = "Logout";
            }
            else
            {
                LoginButton.Content = "Login";
            }
            OnPropertyChanged(nameof(LoginButton));
        }
    }
}
