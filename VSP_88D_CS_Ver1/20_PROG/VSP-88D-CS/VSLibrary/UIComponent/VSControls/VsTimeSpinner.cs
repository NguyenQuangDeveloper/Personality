using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VSLibrary.UIComponent.VSControls;

/// <summary>
/// 시간(시:분:초) 입력 및 Up/Down 증감이 가능한 커스텀 WPF 컨트롤입니다.
/// </summary>
public class VsTimeSpinner : Control
{
    private const string PART_HourBox = "PART_HourBox";
    private const string PART_MinBox = "PART_MinBox";
    private const string PART_SecBox = "PART_SecBox";
    private const string PART_UpButton = "PART_UpButton";
    private const string PART_DownButton = "PART_DownButton";

    private TextBox? _hourBox;
    private TextBox? _minBox;
    private TextBox? _secBox;
    private ButtonBase? _upButton;
    private ButtonBase? _downButton;

    // 현재 포커스 중인 박스
    private enum FocusedBox { Hour, Minute, Second }
    private FocusedBox _activeBox = FocusedBox.Minute;

    /// <summary>
    /// 시간 값 (TimeSpan)
    /// </summary>
    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    /// <summary>
    /// Time DependencyProperty 등록
    /// </summary>
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register(
            nameof(Time),
            typeof(TimeSpan),
            typeof(VsTimeSpinner),
            new FrameworkPropertyMetadata(default(TimeSpan), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged));

    /// <summary>
    /// Time 값 변경시 TextBox에 반영
    /// </summary>
    private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spinner = d as VsTimeSpinner;
        spinner?.UpdateTextBoxes();
    }

    /// <summary>
    /// Up/Down 커맨드 등록
    /// </summary>
    public static RoutedCommand UpCommand = new(nameof(UpCommand), typeof(VsTimeSpinner));
    public static RoutedCommand DownCommand = new(nameof(DownCommand), typeof(VsTimeSpinner));

    static VsTimeSpinner()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(VsTimeSpinner),
            new FrameworkPropertyMetadata(typeof(VsTimeSpinner)));

        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsTimeSpinnerStyle.xaml", UriKind.RelativeOrAbsolute);

        if (Application.Current != null)
        {
            bool alreadyAdded = Application.Current.Resources.MergedDictionaries
                .OfType<ResourceDictionary>()
                .Any(x => x.Source != null && x.Source.Equals(uri));

            if (!alreadyAdded)
            {
                var dict = new ResourceDictionary { Source = uri };
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
    }

    /// <summary>
    /// 생성자 - 커맨드 바인딩 등록
    /// </summary>
    public VsTimeSpinner()
    {
        CommandBindings.Add(new CommandBinding(UpCommand, OnUpExecuted));
        CommandBindings.Add(new CommandBinding(DownCommand, OnDownExecuted));
    }

    /// <summary>
    /// 템플릿 적용 시 파트 연결 및 포커스 이벤트 등록
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _hourBox = GetTemplateChild(PART_HourBox) as TextBox;
        _minBox = GetTemplateChild(PART_MinBox) as TextBox;
        _secBox = GetTemplateChild(PART_SecBox) as TextBox;
        _upButton = GetTemplateChild(PART_UpButton) as ButtonBase;
        _downButton = GetTemplateChild(PART_DownButton) as ButtonBase;

        if (_hourBox != null)
        {
            _hourBox.GotFocus += (_, __) => _activeBox = FocusedBox.Hour;
            _hourBox.LostFocus += HourBox_LostFocus;
            _hourBox.PreviewKeyDown += HourBox_PreviewKeyDown;
        }
        if (_minBox != null)
        {
            _minBox.GotFocus += (_, __) => _activeBox = FocusedBox.Minute;
            _minBox.LostFocus += MinBox_LostFocus;
            _minBox.PreviewKeyDown += MinBox_PreviewKeyDown;
        }
        if (_secBox != null)
        {
            _secBox.GotFocus += (_, __) => _activeBox = FocusedBox.Second;
            _secBox.LostFocus += SecBox_LostFocus;
            _secBox.PreviewKeyDown += SecBox_PreviewKeyDown;
        }

        if (_upButton != null)
            _upButton.Command = UpCommand;
        if (_downButton != null)
            _downButton.Command = DownCommand;

        UpdateTextBoxes();
    }

    /// <summary>
    /// 시간 박스 포커스 해제 또는 엔터 입력시 Time 갱신
    /// </summary>
    private void HourBox_LostFocus(object sender, RoutedEventArgs e) => TryUpdateTimeFromBoxes();
    private void MinBox_LostFocus(object sender, RoutedEventArgs e) => TryUpdateTimeFromBoxes();
    private void SecBox_LostFocus(object sender, RoutedEventArgs e) => TryUpdateTimeFromBoxes();

    private void HourBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) TryUpdateTimeFromBoxes();
    }
    private void MinBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) TryUpdateTimeFromBoxes();
    }
    private void SecBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) TryUpdateTimeFromBoxes();
    }

    /// <summary>
    /// 3개 박스의 텍스트를 읽어서 TimeSpan(Time)에 반영
    /// </summary>
    private void TryUpdateTimeFromBoxes()
    {
        if (_hourBox == null || _minBox == null || _secBox == null) return;

        int h = Clamp(Parse(_hourBox.Text), 0, 23);
        int m = Clamp(Parse(_minBox.Text), 0, 59);
        int s = Clamp(Parse(_secBox.Text), 0, 59);

        Time = new TimeSpan(h, m, s);

        // 항상 박스 텍스트를 Time 기준으로 재동기화 (잘못 입력시)
        UpdateTextBoxes();
    }

    private int Parse(string? s)
    {
        return int.TryParse(s, out var v) ? v : 0;
    }

    private int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Up/Down 버튼 클릭 시 시간, 분, 초의 자리 올림/내림이 적용된 TimeSpan 업데이트를 수행합니다.
    /// </summary>
    private void OnUpExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        switch (_activeBox)
        {
            case FocusedBox.Hour:
                // 23 → 0, 0~22 → +1
                Time = new TimeSpan((Time.Hours + 1) % 24, Time.Minutes, Time.Seconds);
                break;
            case FocusedBox.Minute:
                // 59 → 분 0, 시 +1, 0~58 → +1
                if (Time.Minutes == 59)
                {
                    int newHour = (Time.Hours + 1) % 24;
                    Time = new TimeSpan(newHour, 0, Time.Seconds);
                }
                else
                {
                    Time = new TimeSpan(Time.Hours, Time.Minutes + 1, Time.Seconds);
                }
                break;
            case FocusedBox.Second:
                // 59 → 초 0, 분 +1 (자리올림), 0~58 → +1
                if (Time.Seconds == 59)
                {
                    int newMinute = Time.Minutes + 1;
                    int newHour = Time.Hours;
                    if (newMinute == 60)
                    {
                        newMinute = 0;
                        newHour = (newHour + 1) % 24;
                    }
                    Time = new TimeSpan(newHour, newMinute, 0);
                }
                else
                {
                    Time = new TimeSpan(Time.Hours, Time.Minutes, Time.Seconds + 1);
                }
                break;
        }
    }

    /// <summary>
    /// Down 버튼 클릭 시 시간, 분, 초의 자리 내림/언더플로가 적용된 TimeSpan 업데이트를 수행합니다.
    /// </summary>
    private void OnDownExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        switch (_activeBox)
        {
            case FocusedBox.Hour:
                // 0 → 23, 1~23 → -1
                Time = new TimeSpan((Time.Hours + 23) % 24, Time.Minutes, Time.Seconds);
                break;
            case FocusedBox.Minute:
                // 0 → 분 59, 시 -1 (자리내림), 1~59 → -1
                if (Time.Minutes == 0)
                {
                    int newHour = (Time.Hours + 23) % 24;
                    Time = new TimeSpan(newHour, 59, Time.Seconds);
                }
                else
                {
                    Time = new TimeSpan(Time.Hours, Time.Minutes - 1, Time.Seconds);
                }
                break;
            case FocusedBox.Second:
                // 0 → 초 59, 분 -1 (자리내림), 1~59 → -1
                if (Time.Seconds == 0)
                {
                    int newMinute = Time.Minutes;
                    int newHour = Time.Hours;
                    if (newMinute == 0)
                    {
                        newMinute = 59;
                        newHour = (newHour + 23) % 24;
                    }
                    else
                    {
                        newMinute = newMinute - 1;
                    }
                    Time = new TimeSpan(newHour, newMinute, 59);
                }
                else
                {
                    Time = new TimeSpan(Time.Hours, Time.Minutes, Time.Seconds - 1);
                }
                break;
        }
    }

    /// <summary>
    /// Time 값이 변경되면 각 TextBox에 값 반영
    /// </summary>
    private void UpdateTextBoxes()
    {
        if (_hourBox != null)
            _hourBox.Text = Time.Hours.ToString("D2");
        if (_minBox != null)
            _minBox.Text = Time.Minutes.ToString("D2");
        if (_secBox != null)
            _secBox.Text = Time.Seconds.ToString("D2");
    }
}