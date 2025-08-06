using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.VsNavigations;
using VSP_88D_CS.Views.Report.Sub;

namespace VSP_88D_CS.ViewModels.Report
{
    public partial class ReportPanelViewModel : ViewModelBase
    {
        //private readonly IRegionManager _regionManager;
        //public ICommand ReportMenuBtnCommand { get; }

        //public ReportPanelViewModel(IRegionManager regionManager)
        //{
        //    _regionManager = regionManager;

        //    _regionManager.RequestNavigate<LogPage>("ReportPage");
        //    _regionManager.RequestNavigate<ReportMenu>("ReportMenu");   
        //}

        [ObservableProperty]
        private ObservableCollection<string> _contentList = new ObservableCollection<string>
        {
            "LogPage",        
            "VsPlasmaLog",
            "ChartPage"

        }; 
        [ObservableProperty]
        private string _currentView ;


        [ObservableProperty]
        public ObservableCollection<ButtonData> _buttonDatas = null!;
        public ReportPanelViewModel()
        {
            CurrentView = _contentList[0];
        }
        [RelayCommand]
        private void Log()
        {
            CurrentView = _contentList[0];
        }

        /// <summary>
        /// Command to switch to the Cleaning view.
        /// </summary>
        [RelayCommand]
        private void Chart()
        {
            CurrentView = _contentList[1];
        }

        /// <summary>
        /// Command to switch to the Device view.
        /// </summary>
        [RelayCommand]
        private void MesLog()
        {
            CurrentView = _contentList[2];
        }
        private void SubMenuCreate()
        {
            VsNavigationBar.AutoLogoutOccurred += (sender, e) =>
            {
                ButtonDatas = CreateNavigationButtons();
                CurrentView = _contentList[0];
            };

            ButtonDatas = CreateNavigationButtons();
        }

      
        private ObservableCollection<ButtonData> CreateNavigationButtons()
        {
            var buttons = new ObservableCollection<ButtonData>
            {
                VsNavigationHelper.Create("Log", LogCommand, imagePath: "/Resources/Icons/Log.png", isSelected: true),
                VsNavigationHelper.Create("Chart", ChartCommand, imagePath: "/Resources/Icons/chart.png"),
                VsNavigationHelper.Create("MesLog", MesLogCommand, imagePath: "/Resources/Icons/MesLog.png")
            };
          
            if (buttons.Count > 0)
                buttons[buttons.Count - 1].Margin = new Thickness(0);

            return buttons;
        }
        public override void Activate()
        {
            SubMenuCreate();
        }
    }
}
