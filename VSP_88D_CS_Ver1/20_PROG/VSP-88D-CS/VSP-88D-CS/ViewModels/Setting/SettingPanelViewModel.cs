using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UserAccessLib.Common.Interfaces;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.VsNavigations;
using VSP_88D_CS.Common;
using VSP_88D_CS.ViewModels.Setting.Sub;
using VSP_88D_CS.Views.Setting;
using VSP_88D_CS.Views.Setting.Sub;
using RelayCommand = VSLibrary.Common.MVVM.Core.RelayCommand;

namespace VSP_88D_CS.ViewModels.Setting
{
    public partial class SettingPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// List of content type identifiers corresponding to the views.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> _contentList = new ObservableCollection<string>
        {
            nameof(RecipePage),
            nameof(CleaningPage),
            nameof(DevicePage),
            nameof(ParameterPage),
            nameof(RegisterPage),
            nameof(RegisterPage),
        };

        /// <summary>
        /// Collection of button data used for sub-menu navigation.
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<ButtonData> _buttonDatas = null!;

        private VSContainer _vsContainer => VSContainer.Instance;
        public string SettingRegion => nameof(SettingPanel);


        /// <summary>
        /// Command to switch to the Recipe view.
        /// </summary>
        [RelayCommand]
        private void Recipe()
        {
            _vsContainer.RegionManager.ClearCacheView(SettingRegion, typeof(RecipePage));
            _vsContainer.ClearCache(typeof(RecipePageViewModel));
            _vsContainer.RegionManager.RequestNavigate<RecipePage>(SettingRegion);
        }

        /// <summary>
        /// Command to switch to the Cleaning view.
        /// </summary>
        [RelayCommand]
        private void Cleaning()
        {
            _vsContainer.RegionManager.RequestNavigate<CleaningPage>(SettingRegion);
        }

        /// <summary>
        /// Command to switch to the Device view.
        /// </summary>
        [RelayCommand]
        private void Device()
        {
            _vsContainer.RegionManager.RequestNavigate<DevicePage>(SettingRegion);
        }

        /// <summary>
        /// Command to switch to the Parameter view.
        /// </summary>
        [RelayCommand]
        private void VsParameter()
        {
            _vsContainer.RegionManager.RequestNavigate<ParameterPage>(SettingRegion);
        }

        /// <summary>
        /// Command to switch to the Register view.
        /// </summary>
        [RelayCommand]
        private void Register()
        {
            _vsContainer.RegionManager.RequestNavigate<RegisterPage>(SettingRegion);
        }

        /// <summary>
        /// Initializes the sub-menu buttons if not already created.
        /// </summary>
        private void SubMenuCreate()
        {
            VsNavigationBar.AutoLogoutOccurred += (sender, e) =>
            {
                ButtonDatas = CreateNavigationButtons();
            };

            ButtonDatas = CreateNavigationButtons();
        }

        /// <summary>
        /// 네비게이션 바 버튼 리스트를 생성합니다.
        /// </summary>
        /// <returns>초기화된 ObservableCollection<ButtonData></returns>
        private ObservableCollection<ButtonData> CreateNavigationButtons()
        {
            var buttons = new ObservableCollection<ButtonData>();

            buttons.Add(VsNavigationHelper.Create("Recipe", RecipeCommand, 0, "pack://application:,,,/Resources/Icons/Recipe.png", isSelected: true));
            buttons.Add(VsNavigationHelper.Create("Cleaning", CleaningCommand, 0, "pack://application:,,,/Resources/Icons/Clean.png"));
            buttons.Add(VsNavigationHelper.Create("Device", DeviceCommand, 0, "pack://application:,,,/Resources/Icons/Device.png"));
            buttons.Add(VsNavigationHelper.Create("Parameter", VsParameterCommand, 0, "pack://application:,,,/Resources/Icons/Parameter.png"));
            buttons.Add(VsNavigationHelper.Create("Register", RegisterCommand, 0, "pack://application:,,,/Resources/Icons/Register.png"));

            // 마지막 버튼의 Margin 제거
            if (buttons.Count > 0)
                buttons[buttons.Count - 1].Margin = new Thickness(0);

            return buttons;
        }

        /// <summary>
        /// Called when the view is activated. Initializes the sub-menu.
        /// </summary>
        public override void Activate()
        {
            SubMenuCreate();
        }

        /// <summary>
        /// Called when the view is deactivated. Implement if needed.
        /// </summary>
        public override void Deactivate()
        {
            // Implement if needed
        }


        private readonly IAuthService _authService;
        private readonly IGlobalSystemOption _globalSystemOption;

        public ICommand F6Command { get; }

        public SettingPanelViewModel(IGlobalSystemOption globalSystemOption, IAuthService authService)
        {
            _authService = authService;
            _globalSystemOption = globalSystemOption;

            F6Command = new RelayCommand(OnF6Command);
            Recipe();
        }

        private void OnF6Command()
        {
            _vsContainer.RegionManager.RequestNavigate<SystemParam>(SettingRegion);
        }
    }
}
