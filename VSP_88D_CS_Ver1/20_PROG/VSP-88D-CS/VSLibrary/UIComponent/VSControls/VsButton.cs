using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using VSLibrary.UIComponent.MessageBox;
using VSLibrary.UIComponent.VsLogin;
using VSLibrary.UIComponent.VsNavigations;

namespace VSLibrary.UIComponent.VSControls;

/// <summary>
/// \class VsButton
/// \brief VSLibrary 프레임워크용 커스텀 버튼 클래스입니다.
///
/// PNG, JPG 이미지 또는 벡터 Path 데이터를 사용하여 버튼 아이콘을 표현할 수 있습니다.
/// App.xaml에 스타일을 등록하지 않아도 사용할 수 있도록 ResourceDictionary를 수동 병합합니다.
/// </summary>
public class VsButton : Button
{
    /// <summary>
    /// 아이콘 위치 지정용 열거형 (왼쪽, 오른쪽, 위, 아래)
    /// </summary>
    public enum IconPosition
    {
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>
    /// 이미지 위치 설정용 의존 속성
    /// </summary>
    public IconPosition ImagePosition
    {
        get => (IconPosition)GetValue(ImagePositionProperty);
        set => SetValue(ImagePositionProperty, value);
    }

    public static readonly DependencyProperty ImagePositionProperty =
        DependencyProperty.Register(
            nameof(ImagePosition),
            typeof(IconPosition),
            typeof(VsButton),
            new FrameworkPropertyMetadata(IconPosition.Left, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));


    /// <summary>
    /// VsButton 클래스의 정적 생성자입니다.
    /// 
    /// 기본 스타일 키를 설정하고, XAML 스타일을 수동으로 병합하여
    /// App.xaml에 등록하지 않아도 스타일이 적용되도록 합니다.
    /// </summary>
    static VsButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsButton),
            new FrameworkPropertyMetadata(typeof(VsButton)));

        // 이미 같은 소스의 ResourceDictionary가 등록돼있는지 확인!
        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsButtonStyle.xaml", UriKind.RelativeOrAbsolute);
        bool alreadyAdded = Application.Current.Resources.MergedDictionaries
            .OfType<ResourceDictionary>()
            .Any(x => x.Source != null && x.Source.Equals(uri));

        if (!alreadyAdded)
        {
            var dict = new ResourceDictionary { Source = uri };
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }

    /// <summary>
    /// \property ImageSource
    /// \brief 버튼에 표시할 PNG 또는 JPG 이미지 경로입니다.
    ///
    /// 이 속성은 이미지 리소스를 받아와 버튼의 아이콘으로 사용됩니다.
    /// </summary>
    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    /// ImageSource 의존 속성 정의
    /// </summary>
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(VsButton), new PropertyMetadata(null));

    /// <summary>
    /// \property IconPath
    /// \brief 버튼에 표시할 Path 벡터 아이콘 데이터입니다.
    ///
    /// PathGeometry 또는 StreamGeometry 형식의 데이터를 사용하여 벡터 기반 아이콘을 표현할 수 있습니다.
    /// </summary>
    public Geometry IconPath
    {
        get => (Geometry)GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    /// <summary>
    /// IconPath 의존 속성 정의
    /// </summary>
    public static readonly DependencyProperty IconPathProperty =
        DependencyProperty.Register(nameof(IconPath), typeof(Geometry), typeof(VsButton), new PropertyMetadata(null));

    /// <summary>
    /// \property Command
    /// \brief 지정된 이벤트 발생 시 실행할 ICommand를 정의합니다.
    ///
    /// 이 속성은 AttachedProperty 형태로 정의되며,  
    /// Click, TouchUp, PreviewKeyUp 등 이벤트 발생 시 바인딩된 ICommand를 실행합니다.
    /// </summary>
    public new static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(VsButton),
            new PropertyMetadata(null, OnCommandChanged));

    /// <summary>
    /// \property CommandParameter
    /// \brief Command 실행 시 전달할 파라미터입니다.
    ///
    /// 이 속성이 설정되지 않은 경우, 이벤트 인자(RoutedEventArgs 또는 MouseButtonEventArgs 등)가 자동 전달됩니다.
    /// </summary>
    public new static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(VsButton),
            new PropertyMetadata(null));

    /// <summary>
    /// \property CommandTriggerName
    /// \brief 어떤 이벤트가 발생했을 때 Command를 실행할지 결정하는 이벤트 이름입니다.
    ///
    /// 예: "Click", "PreviewMouseUp", "PreviewKeyUp", "TouchUp" 등  
    /// 설정된 이벤트 이름과 발생 이벤트 이름이 일치할 경우에만 커맨드가 실행됩니다.
    /// </summary>
    public static readonly DependencyProperty CommandTriggerNameProperty =
    DependencyProperty.RegisterAttached(
        "CommandTriggerName",
        typeof(string),
        typeof(VsButton),
        new PropertyMetadata("PreviewMouseUp"));    

    /// <summary>
    /// CommandTriggerName 속성 설정자입니다.
    /// </summary>
    public static void SetCommandTriggerName(DependencyObject obj, string value)
        => obj.SetValue(CommandTriggerNameProperty, value);

    /// <summary>
    /// CommandTriggerName 속성 접근자입니다.
    /// </summary>
    public static string GetCommandTriggerName(DependencyObject obj)
        => (string)obj.GetValue(CommandTriggerNameProperty);

    /// <summary>
    /// \brief Command 속성을 설정합니다.
    /// \param obj 대상 DependencyObject
    /// \param value 바인딩할 ICommand 인스턴스
    /// </summary>
    public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

    /// <summary>
    /// \brief Command 속성을 가져옵니다.
    /// \param obj 대상 DependencyObject
    /// \return 현재 설정된 ICommand
    /// </summary>
    public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);

    /// <summary>
    /// \brief CommandParameter 속성을 설정합니다.
    /// \param obj 대상 DependencyObject
    /// \param value 명령 실행 시 전달할 파라미터
    /// </summary>
    public static void SetCommandParameter(DependencyObject obj, object value)
        => obj.SetValue(CommandParameterProperty, value);

    /// <summary>
    /// \brief CommandParameter 속성을 가져옵니다.
    /// \param obj 대상 DependencyObject
    /// \return 설정된 파라미터 객체
    /// </summary>
    public static object GetCommandParameter(DependencyObject obj)
        => obj.GetValue(CommandParameterProperty);

    /// <summary>
    /// Command 속성 변경 시 이벤트 핸들러를 등록합니다.
    ///
    /// 내부적으로 지원하는 이벤트:
    /// - PreviewMouseUp (MouseButtonEventArgs)
    /// - PreviewKeyDown (실제로는 PreviewKeyUp로 처리됨)
    /// - TouchUp (TouchEventArgs)
    /// - Click (RoutedEventArgs)
    /// 
    /// 실제 이벤트 발생 시 CommandTriggerName 값과 비교하여 실행 여부를 결정합니다.
    /// </summary>
    /// <param name="d">DependencyObject (일반적으로 VsButton)</param>
    /// <param name="e">속성 변경 이벤트 정보</param>
    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element)
        {
            // 마우스 클릭
            element.AddHandler(UIElement.PreviewMouseUpEvent, new MouseButtonEventHandler((s, e) =>
            {
                TryExecuteCommand(d, "PreviewMouseUp", e);
            }), true);
            element.AddHandler(Control.MouseDoubleClickEvent, new MouseButtonEventHandler((s, e) =>
            {
                TryExecuteCommand(d, "MouseDoubleClick", e);
            }), true);

            // 키보드 입력
            element.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler((s, e) =>
            {
                TryExecuteCommand(d, "PreviewKeyUp", e);
            }), true);

            element.AddHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>((s, e) =>
            {
                TryExecuteCommand(d, "TouchUp", e);
            }));

            element.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((s, e) =>
            {
                TryExecuteCommand(d, "Click", e);
            }));
        }
    }

    /// <summary>
    /// \brief 지정된 이벤트 이름이 CommandTriggerName 목록 중 하나와 일치할 경우, Command를 실행합니다.
    ///
    /// CommandTriggerName은 콤마(,)로 구분된 다중 이벤트 이름을 지원하며,  
    /// 실제 발생한 이벤트 이름이 포함된 경우에만 명령이 실행됩니다.
    ///
    /// 예: "Click,MouseDoubleClick,PreviewKeyUp"
    ///
    /// CommandParameter가 지정되어 있으면 해당 값을 전달하고,  
    /// 그렇지 않으면 이벤트 인자(RoutedEventArgs, MouseButtonEventArgs 등)가 자동 전달됩니다.
    /// </summary>
    /// <param name="d">명령 및 트리거 속성이 설정된 객체</param>
    /// <param name="eventName">실제 발생한 이벤트 이름 (예: "Click", "MouseDoubleClick")</param>
    /// <param name="eventArgs">해당 이벤트 인자 객체</param>
    private static async void TryExecuteCommand(DependencyObject d, string eventName, RoutedEventArgs eventArgs)
    {
        var rawTrigger = GetCommandTriggerName(d);
        if (string.IsNullOrEmpty(rawTrigger))
            return;

        // 콤마로 분리된 다중 이벤트 이름 처리
        var triggerList = rawTrigger.Split(',')
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrEmpty(x));

        // 이벤트 이름이 하나라도 일치하면 실행
        if (!triggerList.Contains(eventName, StringComparer.OrdinalIgnoreCase))
            return;

        var command = GetCommand(d);
        var parameter = GetCommandParameter(d) ?? eventArgs;

        if (d is VsButton btn)
        {
            int currentGrade = VsNavigationHelper.CurrentUser?.Grade ?? 0;
            bool forceLogin = !VsNavigationHelper.IsOnceLoginEnabled;
            bool insufficient = currentGrade < btn.MinimumGrade;

            if (!forceLogin)
            {               
                if(btn.Grade < btn.MinimumGrade)
                {
                    VsMessageBox.ShowAsync("권한이 없습니다.\n로그인 후 이용해 주세요.", "알림", autoClick: MessageBoxResult.OK, autoClickDelaySeconds: 3);
                    return;
                }
            }
            else
            {
                if (forceLogin || insufficient)
                {
                    if (!VsLoginControlViewModel.IsShow)
                    {
                        var user = await VsLoginControlViewModel.ShowLoginDialogAsync();
                        if (user == null || user.Grade < btn.MinimumGrade)
                            return;

                        VsNavigationHelper.CurrentUser = user;
                        btn.Grade = user.Grade; // 버튼에도 적용
                    }                 
                }
            }
            var btnData = btn.DataContext as ButtonData;
            VsNavigationBar.NotifyButtonClicked(btnData!);
        }

        if (command?.CanExecute(parameter) == true)
            command.Execute(parameter);
    }

    /// <summary>
    /// \property BackgroundTop
    /// \brief 버튼의 상단 배경 그라데이션 색상입니다.
    ///
    /// 이 색상은 기본 Background와 함께 위쪽 GradientStop에서 사용됩니다.
    /// 설정하지 않으면 Fallback으로 Yellow가 사용됩니다.
    /// </summary>
    public static readonly DependencyProperty BackgroundTopProperty =
        DependencyProperty.Register(nameof(BackgroundTop), typeof(Brush), typeof(VsButton), new PropertyMetadata(null));


    /// <summary>
    /// \property ShineColor
    /// \brief 버튼 상단 샤인 효과의 시작 색상입니다.
    ///
    /// 일반적으로 반짝이는 느낌을 주기 위해 밝은 색 또는 투명도를 가진 색상을 설정합니다.  
    /// 이 값은 ShineColorBottom과 함께 상단에 반짝이는 레이어로 그라데이션 효과를 만듭니다.
    /// </summary>
    public static readonly DependencyProperty ShineColorProperty =
        DependencyProperty.Register(nameof(ShineColor), typeof(Brush), typeof(VsButton), new PropertyMetadata(null));

    /// <summary>
    /// \property ShineColorBottom
    /// \brief 버튼 샤인 효과의 하단 색상입니다.
    ///
    /// ShineColor와 함께 그라데이션 효과를 형성하며,  
    /// 보통은 흰색(White) 또는 약간 어두운 색으로 설정하여 깊이감을 표현합니다.
    /// </summary>
    public static readonly DependencyProperty ShineColorBottomProperty =
        DependencyProperty.Register(nameof(ShineColorBottom), typeof(Brush), typeof(VsButton), new PropertyMetadata(null));

    /// <summary>
    /// 버튼 상단 배경색을 가져오거나 설정합니다.
    /// 해당 색상은 그라데이션 상단 지점에 사용됩니다.
    /// </summary>
    public Brush BackgroundTop
    {
        get => (Brush)GetValue(BackgroundTopProperty);
        set => SetValue(BackgroundTopProperty, value);
    }

    /// <summary>
    /// 샤인 효과의 상단 색상을 가져오거나 설정합니다.
    /// 투명 색상을 사용하는 경우 자연스러운 빛 반사 효과를 제공합니다.
    /// </summary>
    public Brush ShineColor
    {
        get => (Brush)GetValue(ShineColorProperty);
        set => SetValue(ShineColorProperty, value);
    }

    /// <summary>
    /// 샤인 효과의 하단 색상을 가져오거나 설정합니다.
    /// ShineColor와 함께 사용되며 보통 흰색(White) 또는 밝은 계열이 사용됩니다.
    /// </summary>
    public Brush ShineColorBottom
    {
        get => (Brush)GetValue(ShineColorBottomProperty);
        set => SetValue(ShineColorBottomProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty =
    DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(VsButton), new PropertyMetadata(false));

    /// <summary>
    /// 현재 선택된 버튼 여부입니다. 스타일에서 강조 색상 등에 사용됩니다.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsFocusableExProperty =
    DependencyProperty.Register(nameof(IsFocusableEx), typeof(bool), typeof(VsButton), new PropertyMetadata(true));

    /// <summary>
    /// 버튼이 포커스를 받을 수 있는지 여부입니다.
    /// 일반 Focusable은 내부적으로 사용되기 때문에 확장 이름으로 분리합니다.
    /// </summary>
    public bool IsFocusableEx
    {
        get => (bool)GetValue(IsFocusableExProperty);
        set => SetValue(IsFocusableExProperty, value);
    }

    public static readonly DependencyProperty RestoreFocusTargetProperty =
    DependencyProperty.Register(nameof(RestoreFocusTarget), typeof(IInputElement), typeof(VsButton), new PropertyMetadata(null));

    /// <summary>
    /// 버튼 클릭 시 포커스를 복원할 대상입니다.
    /// IsFocusableEx == false인 경우 이 객체로 포커스를 강제로 넘깁니다.
    /// </summary>
    public IInputElement? RestoreFocusTarget
    {
        get => (IInputElement?)GetValue(RestoreFocusTargetProperty);
        set => SetValue(RestoreFocusTargetProperty, value);
    }

    /// <summary>
    /// 사용자의 현재 로그인 등급입니다.
    /// ViewModel 또는 코드에서 설정하여 버튼 권한을 판단할 수 있습니다.
    /// </summary>
    public static readonly DependencyProperty GradeProperty =
        DependencyProperty.Register(nameof(Grade), typeof(int), typeof(VsButton),
            new PropertyMetadata(0, null));

    /// <summary>
    /// 현재 로그인된 사용자의 권한 등급입니다.
    /// </summary>
    public int Grade
    {
        get => (int)GetValue(GradeProperty);
        set => SetValue(GradeProperty, value);
    }

    /// <summary>
    /// 해당 버튼을 클릭할 수 있는 최소 권한 등급입니다.
    /// Grade가 이 값 이상일 경우에만 버튼이 활성화됩니다.
    /// </summary>
    public static readonly DependencyProperty MinimumGradeProperty =
        DependencyProperty.Register(nameof(MinimumGrade), typeof(int), typeof(VsButton),
            new PropertyMetadata(0, null));

    /// <summary>
    /// 버튼이 활성화되기 위한 최소 권한 등급입니다.
    /// </summary>
    public int MinimumGrade
    {
        get => (int)GetValue(MinimumGradeProperty);
        set => SetValue(MinimumGradeProperty, value);
    }    
}
