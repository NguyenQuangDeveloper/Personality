using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VSLibrary.Common.Log;
using VSLibrary.UIComponent.LayoutPanels.CenterPanel.Parameter;
using VSLibrary.UIComponent.Localization;
using VSLibrary.UIComponent.MessageBox;

namespace VSLibrary.UIComponent.LayoutPanels.TopPanel;

/// <summary>
/// VsTopPanel.xaml에 대한 상호 작용 논리
/// </summary>
[INotifyPropertyChanged]
public partial class VsTopPanel : UserControl
{
    /// <summary>
    /// 외부에서 설정 가능한 로고 이미지 소스 (바인딩/직접 할당 모두 지원)
    /// </summary>
    public static readonly DependencyProperty LogoSourceProperty =
        DependencyProperty.Register(
            nameof(LogoSource),
            typeof(ImageSource),
            typeof(VsTopPanel),
            new PropertyMetadata(null));

    /// <summary>
    /// VsTopPanel 좌측 로고 이미지
    /// </summary>
    public ImageSource? LogoSource
    {
        get => (ImageSource?)GetValue(LogoSourceProperty);
        set => SetValue(LogoSourceProperty, value);
    }

    /// <summary>
    /// 장비명(상단 중앙) 바인딩/설정
    /// </summary>
    public static readonly DependencyProperty EquipmentNameProperty =
        DependencyProperty.Register(
            nameof(EquipmentName),
            typeof(string),
            typeof(VsTopPanel),
            new PropertyMetadata("VSP-88D-PRO PLUS"));

    public string EquipmentName
    {
        get => (string)GetValue(EquipmentNameProperty);
        set => SetValue(EquipmentNameProperty, value);
    }

    /// <summary>
    /// 현재 메뉴(메뉴명) 바인딩/설정
    /// </summary>
    public static readonly DependencyProperty CurrentMenuProperty =
        DependencyProperty.Register(
            nameof(CurrentMenu),
            typeof(string),
            typeof(VsTopPanel),
            new PropertyMetadata("Main"));

    public string CurrentMenu
    {
        get => (string)GetValue(CurrentMenuProperty);
        set => SetValue(CurrentMenuProperty, value);
    }

    /// <summary>
    /// 장비 상태(동작 상태) 바인딩/설정
    /// </summary>
    public static readonly DependencyProperty EquipmentStatusProperty =
        DependencyProperty.Register(
            nameof(EquipmentStatus),
            typeof(string),
            typeof(VsTopPanel),
            new PropertyMetadata("IDLE"));

    public string EquipmentStatus
    {
        get => (string)GetValue(EquipmentStatusProperty);
        set => SetValue(EquipmentStatusProperty, value);
    }

    public static readonly DependencyProperty RedLampProperty =
    DependencyProperty.Register(nameof(RedLamp), typeof(bool), typeof(VsTopPanel), new PropertyMetadata(false));
    public bool RedLamp
    {
        get => (bool)GetValue(RedLampProperty);
        set => SetValue(RedLampProperty, value);
    }

    public static readonly DependencyProperty YellowLampProperty =
        DependencyProperty.Register(nameof(YellowLamp), typeof(bool), typeof(VsTopPanel), new PropertyMetadata(false));
    public bool YellowLamp
    {
        get => (bool)GetValue(YellowLampProperty);
        set => SetValue(YellowLampProperty, value);
    }

    public static readonly DependencyProperty GreenLampProperty =
        DependencyProperty.Register(nameof(GreenLamp), typeof(bool), typeof(VsTopPanel), new PropertyMetadata(false));
    public bool GreenLamp
    {
        get => (bool)GetValue(GreenLampProperty);
        set => SetValue(GreenLampProperty, value);
    }

    public static readonly DependencyProperty ExtendControlProperty =
    DependencyProperty.Register(nameof(ExtendControl), typeof(UIElement), typeof(VsTopPanel), new PropertyMetadata(null));
    public UIElement ExtendControl
    {
        get => (UIElement)GetValue(ExtendControlProperty);
        set => SetValue(ExtendControlProperty, value);
    }

    private DispatcherTimer _timer;

    [ObservableProperty]
    private string _dateString = "";

    [ObservableProperty]
    private string _appVersion = "";

    [ObservableProperty]
    private string _timeString = "";

    public VsTopPanel()
    {
        InitializeComponent();
        // DataContext가 지정되어 있지 않으면, 전역 싱글턴 자동 할당
        if (DataContext == null)
            DataContext = EquipmentStateService.Instance;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) => UpdateDateTime(VsLocalizationManager.CurrentLanguage);
        _timer.Start();

        // 초기 값 세팅

        AppVersion = GetLocalizedVersionText(VsLocalizationManager.CurrentLanguage);
        EquipmentStateService.Instance.EquipmentStatus = VsLocalizationManager.Get(VsLocalizationManager.CurrentLanguage, "VsNavigations", EquipmentStateService.Instance.EquipmentStatus);
        EquipmentStateService.Instance.CurrentMenu = VsLocalizationManager.Get(VsLocalizationManager.CurrentLanguage, "VsNavigations", EquipmentStateService.Instance.CurrentMenu);

        VsParameterData.LanguageChanged += (sender, args) =>
        {
            if (Enum.TryParse(args.ToString(), out LanguageType newLanguage))
            {
                string equipmentStatusKey = VsLocalizationManager.GetKeyFromValue(VsLocalizationManager.CurrentLanguage, "VsNavigations", EquipmentStateService.Instance.EquipmentStatus)!;
                string currentMenuKey = VsLocalizationManager.GetKeyFromValue(VsLocalizationManager.CurrentLanguage, "VsNavigations", EquipmentStateService.Instance.CurrentMenu)!;
                VsLocalizationManager.CurrentLanguage = newLanguage;
                if (equipmentStatusKey != null || currentMenuKey != null)
                {
                    EquipmentStateService.Instance.EquipmentStatus = VsLocalizationManager.Get(newLanguage, "VsNavigations", equipmentStatusKey!);
                    EquipmentStateService.Instance.CurrentMenu = VsLocalizationManager.Get(newLanguage, "VsNavigations", currentMenuKey!);
                }
                AppVersion = GetLocalizedVersionText(newLanguage);
                UpdateDateTime(newLanguage);
            }

        };
    }

    /// <summary>
    /// 현재 언어에 따라 버전 문자열을 반환합니다.
    /// </summary>
    /// <returns>다국어 버전 문자열</returns>
    private string GetLocalizedVersionText(LanguageType type = LanguageType.English)
    {
        var ver = Assembly.GetEntryAssembly()?.GetName().Version;
        string version = ver?.ToString() ?? "1.0.0.0";

        switch (type)
        {
            case LanguageType.Korean: // 한국어
                return $"버전 {version}";
            case LanguageType.Chinese: // 중국어 (간체 기준)
                return $"版本 {version}";
            case LanguageType.Vietnamese: // 베트남어
                return $"Phiên bản {version}";
            case LanguageType.English: // 영어
            default:
                return $"Version {version}";
        }
    }

    /// <summary>
    /// 언어별로 날짜/시간 문자열을 업데이트합니다.
    /// </summary>
    /// <param name="type">언어 타입</param>
    private void UpdateDateTime(LanguageType type = LanguageType.English)
    {
        var now = DateTime.Now;
        CultureInfo culture;
        string dateFormat;
        string timeFormat = "HH:mm:ss";

        switch (type)
        {
            case LanguageType.Korean:
                culture = new CultureInfo("ko-KR");
                dateFormat = "yyyy-MM-dd (dddd)";
                break;
            case LanguageType.Chinese:
                culture = new CultureInfo("zh-CN");
                dateFormat = "yyyy年MM月dd日 (dddd)";
                break;
            case LanguageType.Vietnamese:
                culture = new CultureInfo("vi-VN");
                dateFormat = "dd/MM/yyyy (dddd)";
                break;
            case LanguageType.English:
            default:
                culture = new CultureInfo("en-US");
                dateFormat = "MMMM dd, yyyy (dddd)";
                break;
        }

        DateString = now.ToString(dateFormat, culture);
        TimeString = now.ToString(timeFormat, culture);
    }

    private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
#if DEBUG
            var win = Window.GetWindow(this);
            if (win != null)
            {
                // 최대화 <-> 복원 토글
                if (win.WindowState == WindowState.Maximized)
                    win.WindowState = WindowState.Normal;
                else
                    win.WindowState = WindowState.Maximized;
            }
#endif
        }
    }
}

public partial class EquipmentStateService : ObservableObject
{
    public static EquipmentStateService Instance { get; } = new EquipmentStateService();

    [ObservableProperty]
    private string _equipmentName = "VSP-88D-PRO PLUS";

    [ObservableProperty]
    private string _currentMenu = "Main";

    [ObservableProperty]
    private string _equipmentStatus = "IDLE";

    [ObservableProperty]
    private bool _redLamp;

    [ObservableProperty]
    private bool _yellowLamp;


    [ObservableProperty]
    private bool _greenLamp;

    private EquipmentStateService() { }

    partial void OnEquipmentStatusChanged(string? oldValue, string newValue)
    {
        var lang = VsLocalizationManager.CurrentLanguage;

        var oldText = VsLocalizationManager.Get(lang, "VsNavigations", oldValue ?? string.Empty) ?? oldValue;
        var newText = VsLocalizationManager.Get(lang, "VsNavigations", newValue) ?? newValue;

        // 메뉴 상태 업데이트
        EquipmentStateService.Instance.EquipmentStatus = newText;

        LogManager.Write($"🟡 상태 변경: [{oldValue}] ➜ [{newValue}] / 표시: '{oldText}' ➜ '{newText}'", LogType.Info);
    }
}