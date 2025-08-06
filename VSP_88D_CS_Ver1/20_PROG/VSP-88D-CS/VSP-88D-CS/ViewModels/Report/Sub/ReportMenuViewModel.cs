using System.Windows.Controls;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Styles.Controls;
using VSP_88D_CS.Views.Report.Sub;

namespace VSP_88D_CS.ViewModels.Report.Sub
{
    public class ReportMenuViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        private readonly IRegionManager _regionManager;
        public ICommand ReportMenuBtnCommand { get; }

        public ReportMenuViewModel(IRegionManager regionManager)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _regionManager = regionManager;
            ReportMenuBtnCommand = new RelayCommand<object>(OnReportMenuBtn);
            _btnLogInfo= new ButtonInfo { Key = "Log", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/log.png" };
            _btnChartInfo= new ButtonInfo { Key = "Chart", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/chart.png" };
            //  IsSelected=true;
        }

        public bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public ButtonInfo _btnLogInfo;

        public ButtonInfo BtnLogInfo
        {
            get { return _btnLogInfo; }
            set { SetProperty(ref _btnLogInfo, value); }
        }
        public ButtonInfo _btnChartInfo;

        public ButtonInfo BtnChartInfo
        {
            get { return _btnChartInfo; }
            set { SetProperty(ref _btnChartInfo, value); }
        }


        private void OnReportMenuBtn(object obj)
        {
           // BtnLogInfo = new ButtonInfo { Key = "Log", IsSelected = true, ImagePath = "pack://application:,,,/Resources/Icons/log.png" };

            var _button = obj as Button;           
            var _btnName = _button.Tag as ButtonInfo;
            BtnLogInfo.IsSelected = false;
            BtnChartInfo.IsSelected = false;
            switch (_btnName.Key)
            {
                case "Log":
                    BtnLogInfo.IsSelected = true;
                    _regionManager.RequestNavigate<LogPage>("ReportPage");
                    break;
                case "Chart":
                    BtnChartInfo.IsSelected = true;
                    _regionManager.RequestNavigate<ChartPage>("ReportPage");
                    break;
                default:
                    _regionManager.RequestNavigate<LogPage>("ReportPage");
                    break;
            };

           
        }
    }
}
