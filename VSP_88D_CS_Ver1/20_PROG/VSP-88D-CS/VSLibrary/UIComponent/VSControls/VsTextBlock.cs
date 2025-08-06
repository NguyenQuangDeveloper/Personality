using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VSLibrary.UIComponent.VSControls;

/// <summary>
/// \class VsTextBlock
/// \brief VSLibrary에서 사용하는 커스텀 TextBlock 컨트롤입니다.
/// 
/// 기본 WPF TextBlock을 상속받아, 공통 스타일 적용 및 자동 리소스 병합 기능을 제공합니다.
/// App.xaml에 스타일을 등록하지 않아도 사용할 수 있도록 설계되어 있습니다.
/// </summary>
public class VsTextBlock : TextBlock
{
    /// <summary>
    /// VsTextBlock 클래스의 static 생성자입니다.
    /// 
    /// - 기본 스타일 키를 VsTextBlock에 맞게 재정의합니다.
    /// - VsTextBlockStyle.xaml 리소스를 전역에 병합하여 자동으로 스타일이 적용되도록 합니다.
    /// </summary>
    static VsTextBlock()
    {
        // 기본 스타일 키 오버라이드
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsTextBlock),
            new FrameworkPropertyMetadata(typeof(VsTextBlock)));

        // 리소스 중복 등록 방지
        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsTextBlockStyle.xaml", UriKind.RelativeOrAbsolute);

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
    public static readonly DependencyProperty CommandProperty =
                              DependencyProperty.RegisterAttached(
                                  "Command",
                                  typeof(ICommand),
                                  typeof(VsTextBlock),
                                  new PropertyMetadata(null, OnCommandChanged));

    /// <summary>
    /// \property CommandParameter
    /// \brief Command 실행 시 전달할 파라미터입니다.
    ///
    /// 이 속성이 설정되지 않은 경우, 이벤트 인자(RoutedEventArgs 또는 MouseButtonEventArgs 등)가 자동 전달됩니다.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(VsTextBlock),
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
        typeof(VsTextBlock),
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
    /// <param name="d">DependencyObject (일반적으로 VsTextBlock)</param>
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
