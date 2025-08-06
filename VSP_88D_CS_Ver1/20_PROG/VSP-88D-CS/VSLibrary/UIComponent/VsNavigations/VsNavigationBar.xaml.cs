using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VSLibrary.Common.Log;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.UIComponent.LayoutPanels.TopPanel;
using VSLibrary.UIComponent.Localization;
using VSLibrary.UIComponent.MessageBox;
using VSLibrary.UIComponent.VSControls;
using VSLibrary.UIComponent.VsLogin;
using VSLibrary.UIComponent.VsLogin.Repository;

namespace VSLibrary.UIComponent.VsNavigations;

public class DesignButtonDatas : ObservableCollection<ButtonData>
{
    public DesignButtonDatas()
    {
        Add(new ButtonData { Content = "Login" });
        Add(new ButtonData { Content = "Main" });
        Add(new ButtonData { Content = "Manual" });
        Add(new ButtonData { Content = "Setting" });
        Add(new ButtonData { Content = "Register" });
        Add(new ButtonData { Content = "About", Visibility = Visibility.Hidden });
        Add(new ButtonData { Content = "About", Visibility = Visibility.Hidden });
        Add(new ButtonData { Content = "About", Visibility = Visibility.Hidden });
        Add(new ButtonData { Content = "About", Visibility = Visibility.Hidden });
        Add(new ButtonData { Content = "Exit" });

        this[Count - 1].Margin = new Thickness(0);
    }
}

/// <summary>
/// VsNavigationBar.xaml에 대한 상호 작용 논리
/// </summary>
public partial class VsNavigationBar : UserControl
{
    /// <summary>
    /// \brief 자동 로그아웃 이벤트가 발생했을 때 호출되는 이벤트입니다.
    /// </summary>
    public static event EventHandler? AutoLogoutOccurred;

    /// <summary>
    /// 자동 로그아웃 타임아웃 (초 단위)
    /// 0일 경우 무제한 대기
    /// </summary>
    public int AutoLogoutTimeout
    {
        get => (int)GetValue(AutoLogoutTimeoutProperty);
        set => SetValue(AutoLogoutTimeoutProperty, value);
    }

    public static readonly DependencyProperty AutoLogoutTimeoutProperty =
        DependencyProperty.Register(
            nameof(AutoLogoutTimeout),
            typeof(int),
            typeof(VsNavigationBar),
            new PropertyMetadata(0, OnAutoLogoutTimeoutChanged));

    private static void OnAutoLogoutTimeoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsNavigationBar nav)
        {
            nav.ResetLogoutTimerIfNeeded();
        }
    }


    private static readonly HashSet<VsNavigationBar> _instances = new();
    private static bool _isInputHooked = false;

    private static void EnsureGlobalInputHook()
    {
        if (_isInputHooked == false)
        {
            InputManager.Current.PreProcessInput += OnGlobalInputStatic;
            _isInputHooked = true;
        }
    }

    private static void OnGlobalInputStatic(object sender, PreProcessInputEventArgs e)
    {
        if (e.StagingItem.Input is MouseEventArgs or KeyboardEventArgs or TouchEventArgs)
        {
            foreach (var nav in _instances)
            {
                if (nav.AutoLogoutTimeout > 0)
                    nav.InitializeOrResetLogoutTimer();
            }
        }
    }

    private DispatcherTimer? _logoutTimer;

    private void ResetLogoutTimerIfNeeded()
    {
        if (AutoLogoutTimeout > 0)
            InitializeOrResetLogoutTimer();
        else
            StopLogoutTimer();
    }

    /// <summary>
    /// 로그아웃 타이머를 초기화하거나 다시 시작합니다.
    /// </summary>
    public void InitializeOrResetLogoutTimer()
    {
        if (VsNavigationHelper.CurrentUser == null || VsNavigationHelper.IsOnceLoginEnabled == false)
            return; // 로그인 안돼 있으면 무시

        if (_logoutTimer == null)
        {
            _logoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(AutoLogoutTimeout)
            };
            _logoutTimer.Tick += (s, e) =>
            {
                _logoutTimer.Stop();
                OnAutoLogout();
            };
        }

        _logoutTimer.Stop();
        _logoutTimer.Interval = TimeSpan.FromSeconds(AutoLogoutTimeout);
        _logoutTimer.Start();
    }

    /// <summary>
    /// 로그아웃 타이머를 중지합니다.
    /// </summary>
    private void StopLogoutTimer()
    {
        _logoutTimer?.Stop();
    }

    /// <summary>
    /// 로그아웃 시 동작을 정의합니다.
    /// </summary>
    private void OnAutoLogout()
    {
        foreach (var btn in ButtonDatas)
        {
            btn.Grade = 0;
        }

        VsNavigationHelper.CurrentUser = null;

        AutoLogoutOccurred?.Invoke(this, EventArgs.Empty);

        VsMessageBox.ShowAsync(
            "사용자 입력이 일정 시간 없어 자동으로 로그아웃 되었습니다.",
            "자동 로그아웃",
            MessageBoxButton.OK,
            MessageBoxImage.Information,
            autoClick: MessageBoxResult.OK,
            autoClickDelaySeconds: 5);
    }

    /// <summary>
    /// 로그인 체크를 한 번만 수행할지 여부 (기본값: false).
    /// true일 경우, 로그인에 성공한 이후엔 더 이상 권한 부족 시에도 로그인창을 띄우지 않음.
    /// </summary>
    public bool OnceLogin
    {
        get => (bool)GetValue(OnceLoginProperty);
        set => SetValue(OnceLoginProperty, value);
    }

    public static readonly DependencyProperty OnceLoginProperty =
        DependencyProperty.Register(
            nameof(OnceLogin),
            typeof(bool),
            typeof(VsNavigationBar),
            new PropertyMetadata(false, OnOnceLoginChanged));

    private static void OnOnceLoginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsNavigationBar nav)
        {
            VsNavigationHelper.IsOnceLoginEnabled = (bool)e.NewValue;
        }
    }

    private static readonly IContainer _container = null!;
    private UserItem _user = null!; // 현재 사용자 정보 (로그인 후 세팅)

    static VsNavigationBar()
    {
        _container = VSContainer.Instance;      
    }

    public VsNavigationBar()
    {
        InitializeComponent();
             
        if (ButtonDatas == null)
            ButtonDatas = new ObservableCollection<ButtonData>();

        var loginControl = _container.Resolve<VsLoginControlViewModel>();

        loginControl.LoginChanged -= OnLoginChanged;
        loginControl.LoginChanged += OnLoginChanged;

        EnsureGlobalInputHook();
        _instances.Add(this);
        Unloaded += (_, _) => _instances.Remove(this);
    }

    private void OnLoginChanged(UserItem? user)
    {
        if (user == null) return;

        _user = user;
        VsNavigationHelper.CurrentUser = user;

        if (OnceLogin == true)
        {
            VsNavigationHelper.IsOnceLoginEnabled = OnceLogin;
        }

        // 모든 버튼 상태 갱신
        foreach (var btn in ButtonDatas)
        {
            btn.Grade = _user.Grade;           
        }
    }

    public LanguageType LanguageType
    {
        get => (LanguageType)GetValue(LanguageTypeProperty);
        set => SetValue(LanguageTypeProperty, value);
    }

    public static readonly DependencyProperty LanguageTypeProperty =
        DependencyProperty.Register(nameof(LanguageType), typeof(LanguageType), typeof(VsNavigationBar), new PropertyMetadata(LanguageType.English));

    /// <summary>
    /// 버튼 데이터 리스트
    /// </summary>
    public ObservableCollection<ButtonData> ButtonDatas
    {
        get => (ObservableCollection<ButtonData>)GetValue(ButtonDatasProperty);
        set => SetValue(ButtonDatasProperty, value);
    }

    /// <summary>
    /// ButtonDatas DependencyProperty
    /// </summary>
    public static readonly DependencyProperty ButtonDatasProperty =
        DependencyProperty.Register(
            nameof(ButtonDatas),
            typeof(ObservableCollection<ButtonData>),
            typeof(VsNavigationBar),
            new PropertyMetadata(null, OnButtonDatasChanged));

    /// <summary>
    /// ButtonDatas 프로퍼티 변경시, Margin 업데이트 및 이벤트 재구독
    /// </summary>
    private static void OnButtonDatasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsNavigationBar nav)
        {
            if (e.OldValue is ObservableCollection<ButtonData> oldList)
                oldList.CollectionChanged -= nav.ButtonDatas_CollectionChanged;

            if (e.NewValue is ObservableCollection<ButtonData> newList)
                newList.CollectionChanged += nav.ButtonDatas_CollectionChanged;

            nav.UpdateMarginAndAlignment();
        }
    }

    /// <summary>
    /// 버튼 데이터 추가/삭제 시 Margin/정렬 업데이트
    /// </summary>
    private void ButtonDatas_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateMarginAndAlignment();
    }

    /// <summary>
    /// 네비게이션 바 표시 방향 (Top/Bottom/Left/Right)
    /// </summary>
    public NavigationBarPosition Position
    {
        get => (NavigationBarPosition)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Position DependencyProperty
    /// </summary>
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register(
            nameof(Position),
            typeof(NavigationBarPosition),
            typeof(VsNavigationBar),
            new PropertyMetadata(NavigationBarPosition.Top, OnPositionChanged));

    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsNavigationBar nav)
            nav.UpdateMarginAndAlignment();
    }

    /// <summary>
    /// 방향 및 보이는 버튼 기준으로 Margin/정렬 자동 재설정
    /// </summary>
    private void UpdateMarginAndAlignment()
    {
        Thickness defaultMargin = Position switch
        {
            NavigationBarPosition.Right => new Thickness(0, 0, 0, 4),
            NavigationBarPosition.Left => new Thickness(0, 0, 0, 4),
            _ => new Thickness(0, 0, 4, 0)
        };
        Thickness lastMargin = Position switch
        {
            NavigationBarPosition.Right => new Thickness(0, 0, 0, 4),
            NavigationBarPosition.Left => new Thickness(0, 0, 0, 4),
            _ => new Thickness(0)
        }; 

        ApplyLastButtonMargin(defaultMargin, lastMargin);       
    }

    /// <summary>
    /// 보이는 마지막 버튼의 Margin만 lastMargin으로, 나머지는 defaultMargin으로 변경
    /// </summary>
    private void ApplyLastButtonMargin(Thickness defaultMargin, Thickness lastMargin)
    {
        if (ButtonDatas == null || ButtonDatas.Count == 0)
            return;

        var visibleIndices = ButtonDatas
            .Select((btn, idx) => new { btn, idx })
            .Where(x => x.btn.Visibility == Visibility.Visible)
            .Select(x => x.idx)
            .ToList();

        if (visibleIndices.Count == 0)
            return;

        foreach (var btn in ButtonDatas)
        {        
            btn.Margin = defaultMargin;
        }
        ButtonDatas[visibleIndices.Last()].Margin = lastMargin;       
    }

    /// <summary>
    /// 버튼 클릭 시 Navigation 상태 및 로그 처리
    /// </summary>
    public static void NotifyButtonClicked(ButtonData clickedButton)
    {
        var navBars = Application.Current.Windows
            .OfType<Window>()
            .SelectMany(w => FindVisualChildren<VsNavigationBar>(w))
            .ToList();

        ButtonData buttonData = null!;

        foreach (var nav in navBars)
        {
            if (!nav.ButtonDatas.Contains(clickedButton))
                continue;

            foreach (var btn in nav.ButtonDatas)
            {
                if (btn.IsSelected == true && btn.IsFocusableEx == true)
                {
                    buttonData = btn;
                }
                btn.IsSelected = false;
            }
            var localizedMenu = VsLocalizationManager.Get(
                VsLocalizationManager.CurrentLanguage,
                "VsNavigations",
                clickedButton.Content) ?? clickedButton.Content;

            LogManager.Write(
                $"🚀 버튼 클릭됨: 원본 = '{clickedButton.Content}', 언어 = {VsLocalizationManager.CurrentLanguage}, 표시 = '{localizedMenu}'",
                LogType.Info);

            if (clickedButton.IsFocusableEx)
            {
                clickedButton.IsSelected = true;
                EquipmentStateService.Instance.CurrentMenu = localizedMenu;
            }
            else
            {

                foreach (var btn in nav.ButtonDatas)
                {
                    if (btn == buttonData)
                    {
                        btn.IsSelected = true;
                    }                    
                }
            }
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null)
            yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

            if (child is T t)
                yield return t;

            foreach (var childOfChild in FindVisualChildren<T>(child))
                yield return childOfChild;
        }
    }

    private void VsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is VsButton button && button.DataContext is ButtonData btnData)
        {
            // 현재 사용자 등급
            int currentGrade = _user !=null ? _user.Grade: 0;

            // ▶️ 권한 체크
            if (currentGrade < btnData.MinimumGrade)
            {                
                // 🔐 권한 없음 → 로그인 창 띄우기                
                return;
            }
        }
    }
}

/// <summary>
/// 네비게이션 바 표시 방향
/// </summary>
public enum NavigationBarPosition
{
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
/// 네비게이션바 동적 버튼 데이터 구조체 (확장 버전)
/// </summary>
public partial class ButtonData : ObservableObject
{
    /// <summary>버튼에 표시할 텍스트</summary>
    [ObservableProperty]
    private string _content = string.Empty;

    /// <summary>아이콘 이미지 경로 (string으로만 저장)</summary>
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>아이콘 이미지 (pack URI 또는 절대경로 지원)</summary>
    public ImageSource ImageSource
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ImagePath))
                return null!;
            try
            {
                return new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return null!;
            }
        }
    }

    /// <summary>버튼 텍스트 색상</summary>
    [ObservableProperty]
    private Brush _foreground = Brushes.Black;

    /// <summary>버튼 상단 샤인 효과 색상</summary>
    [ObservableProperty]
    private Brush _shineColor = Brushes.LightBlue;

    /// <summary>버튼 하단 샤인 효과 색상</summary>
    [ObservableProperty]
    private Brush _shineColorBottom = Brushes.White;

    /// <summary>상단 그라데이션 배경색</summary>
    [ObservableProperty]
    private Brush _backgroundTop = Brushes.Wheat;

    /// <summary>버튼 배경색</summary>
    [ObservableProperty]
    private Brush _background = Brushes.DarkBlue;

    /// <summary>아이콘 위치(왼쪽/오른쪽/위/아래)</summary>
    public VsButton.IconPosition ImagePosition { get; set; } = VsButton.IconPosition.Left;

    /// <summary>버튼 활성화 여부</summary>
    [ObservableProperty]
    private bool _isEnabled = true;

    /// <summary>글꼴 굵기</summary>
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;

    /// <summary>버튼 표시 여부</summary>
    public Visibility Visibility { get; set; } = Visibility.Visible;

    /// <summary>버튼 마진</summary>
    public Thickness Margin { get; set; } = new Thickness(0, 0, 4, 0);

    /// <summary>명령 실행 시 전달할 매개변수</summary>
    public object CommandParameter { get; set; } = null!;

    /// <summary>명령 실행 타겟</summary>
    public IInputElement CommandTarget { get; set; } = null!;

    [ObservableProperty]
    private ICommand? _command;

    /// <summary>현재 선택된 버튼 여부 (시각적 효과용)</summary>
    [ObservableProperty]
    private bool _isSelected = false;

    /// <summary>포커스 가능 여부</summary>
    [ObservableProperty]
    private bool _isFocusableEx = true;

    /// <summary>사용자 권한 등급</summary>
    [ObservableProperty]
    private int _grade = 0;

    /// <summary>이 버튼을 사용할 수 있는 최소 등급</summary>
    [ObservableProperty]
    private int _minimumGrade = 0;
}

/// <summary>
/// VsNavigation 관련 유틸리티 (로그인 상태/옵션 등 관리)
/// </summary>
public static class VsNavigationHelper
{
    /// <summary>
    /// true일 경우, 로그인 성공 시 이후에는 권한이 충분하면 로그인창을 띄우지 않음.
    /// false일 경우, 권한이 충분해도 로그인창을 계속 띄움 (보안 강화 목적).
    /// </summary>
    public static bool IsOnceLoginEnabled { get; set; } = false;

    /// <summary>
    /// OnceLogin이 true일 경우, 로그인 성공 시 저장되는 유저 정보
    /// </summary>
    public static UserItem? CurrentUser { get; set; } = null;

    /// <summary>
    /// 공통 스타일이 적용된 버튼을 생성합니다.
    /// </summary>
    public static ButtonData Create(
        string content,
        ICommand? command = null,
        int minGrade = 0,
        string? imagePath = null,        
        bool isSelected = false,
        bool isFocusable = true,
        Visibility visibility = Visibility.Visible,
        VsButton.IconPosition imagePosition = VsButton.IconPosition.Top)
    {
        return new ButtonData
        {
            Content = content,
            Command = command,
            MinimumGrade = minGrade,
            ImagePath = imagePath!,
            ImagePosition = imagePosition,
            FontWeight = FontWeights.Bold,
            Foreground = GetColor("#143a5a"),
            ShineColor = GetColor("#104E8B"),
            ShineColorBottom = Brushes.White,
            BackgroundTop = Brushes.White,
            Background = Brushes.LightGray,
            IsSelected = isSelected,
            IsFocusableEx = isFocusable,
            Visibility = visibility
        };
    }

    /// <summary>
    /// HEX 색상 문자열을 SolidColorBrush로 변환합니다.
    /// </summary>
    private static SolidColorBrush GetColor(string hex)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }
}
