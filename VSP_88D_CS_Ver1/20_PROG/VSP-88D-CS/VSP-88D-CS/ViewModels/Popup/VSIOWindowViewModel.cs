using System.Windows;
using System.Windows.Media;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.ViewModels.Setting.Sub;

namespace VSP_88D_CS.ViewModels.Popup;

public class VSIOWindowViewModel : ViewModelBase
{
    public string MainTiile { get; set; } = "Digital I/O Monitoring";

    public string DigitalText { get; set; } = "Digital I/O";
    public string AnalogText { get; set; } = "Analog I/O";
    public string CloseText { get; set; } = "Close";


    private readonly IRegionManager _regionManager;
    public RelayCommand<string> NavigateCommand { get; private set; }
    public RelayCommand CloseCommand { get; }
    public RelayCommand TextBlockDoubleClickCommand { get; }

    private Brush _backgroundColor;

    public Brush BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    //private void OnLanguageChanged(string languageKey)
    //{
    //    ChangeLanguage(languageKey);
    //}

    private ParameterPageViewModel _para;

    // 창을 닫기 위한 Action
    public Action CloseAction { get; set; }
    public VSIOWindowViewModel() 
    {
        _regionManager = VSContainer.Instance.RegionManager;
        _para = VSContainer.Instance.Resolve<ParameterPageViewModel>();
        //_para.LanguageChanged += OnLanguageChanged;

        BackgroundColor = Brushes.Gray; // 초기 배경색 설정

        NavigateCommand = new RelayCommand<string>(Navigate);
        // 닫기 명령 초기화
        CloseCommand = new RelayCommand(ExecuteCloseCommand);
        TextBlockDoubleClickCommand = new RelayCommand(OnTextBlockDoubleClick);

        //ParameterItem lang = _para.ParameterItems.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "LANG");
        //long index = Convert.ToInt32(lang.Value);
        //string languageKey = index switch
        //{
        //    0 => "Kor",
        //    1 => "Eng",
        //    2 => "Use1",
        //    3 => "Use2",
        //    _ => "Eng"
        //};
        //ChangeLanguage(languageKey);

        _regionManager.RequestNavigate("IORegion", "DigitalIO");
    }

    public void Navigate(string viewName)
    {
        _regionManager.RequestNavigate("IORegion", viewName);
    }

    private void ExecuteCloseCommand()
    {
        Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
    }

    private void OnTextBlockDoubleClick()
    {
        // 더블 클릭 시 배경색을 변경
        if (BackgroundColor == Brushes.Gray)
        {
            BackgroundColor = Brushes.Teal;
        }
        else
        {
            BackgroundColor = Brushes.Gray;
        }
    }
}
