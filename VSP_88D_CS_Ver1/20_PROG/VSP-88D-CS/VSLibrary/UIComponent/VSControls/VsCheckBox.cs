using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VSLibrary.UIComponent.VSControls;


/// <summary>
/// \class VsCheckBox
/// \brief VSLibrary 전용 커스텀 CheckBox 컨트롤입니다.
///
/// 기본 CheckBox를 확장하여 MVVM 스타일의 명령(Command) 바인딩을 지원하며,  
/// 특정 이벤트(Click, KeyUp, Touch 등)가 발생했을 때 지정된 ICommand를 실행할 수 있도록 구현된 컨트롤입니다.  
/// 또한 XAML 스타일 리소스를 App.xaml 없이 자동 병합 처리합니다.
/// </summary>
public class VsCheckBox : CheckBox
{
    /// <summary>
    /// VsCheckBox 생성자입니다.
    ///
    /// 기본 스타일 키를 재정의하고,  
    /// /UIComponent/Styles/VsCheckBoxStyle.xaml 경로의 리소스를 수동으로 병합하여  
    /// 외부 App.xaml에 별도 등록 없이 스타일 적용이 가능하도록 처리합니다.
    /// </summary>
    static VsCheckBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsCheckBox),
            new FrameworkPropertyMetadata(typeof(VsCheckBox)));

        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsCheckBoxStyle.xaml", UriKind.RelativeOrAbsolute);

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
        typeof(VsCheckBox),
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
        typeof(VsCheckBox),
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
        typeof(VsCheckBox),
        new PropertyMetadata(null));

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
    /// <param name="d">DependencyObject (일반적으로 VsCheckBox)</param>
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
    private static void TryExecuteCommand(DependencyObject d, string eventName, RoutedEventArgs eventArgs)
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

        if (command?.CanExecute(parameter) == true)
            command.Execute(parameter);
    }
}
