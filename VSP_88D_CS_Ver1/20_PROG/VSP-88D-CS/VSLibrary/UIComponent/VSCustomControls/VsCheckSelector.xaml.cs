using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VSLibrary.UIComponent.LayoutPanels.CenterPanel.Parameter;
using VSLibrary.UIComponent.Localization;

namespace VSLibrary.UIComponent.VSCustomControls;

/// <summary>
/// VsCheckSelector.xaml에 대한 상호 작용 논리
/// </summary>
public partial class VsCheckSelector : UserControl
{
    // -- 마지막 선택 라디오 (Left/Right) 상태 기억
    private bool _lastLeftChecked = true;
    private bool _lastRightChecked = false;

    public event RoutedEventHandler ValueChanged = null!;

    public VsCheckSelector()
    {
        InitializeComponent();

        PART_CheckBox.Checked += (s, e) => OnCheckedChanged(true);
        PART_CheckBox.Unchecked += (s, e) => OnCheckedChanged(false);

        PART_RadioLeft.Checked += (s, e) => { RememberRadio(true); RaiseValueChanged(); };
        PART_RadioRight.Checked += (s, e) => { RememberRadio(false); RaiseValueChanged(); };

        PART_AxisMaxBox.LostFocus += (s, e) => RaiseValueChanged();
        PART_AxisMaxBox.KeyDown += (s, e) => { if (((KeyEventArgs)e).Key == Key.Enter) RaiseValueChanged(); };     
    }

    /// <summary>
    /// 체크 변경시 라디오 자동 복원/초기화
    /// </summary>
    private void OnCheckedChanged(bool isChecked)
    {
        if (isChecked)
        {
            // 외부에서 하나라도 True라면(코드/바인딩에서 설정된 경우)
            if (IsLeftChecked || IsRightChecked)
            {
                // 현재 값을 내부 상태로 동기화 (외부에서 설정된 값을 우선)
                _lastLeftChecked = IsLeftChecked;
                _lastRightChecked = IsRightChecked;
            }
            else
            {
                // 둘 다 False면 기억값 복원
                IsLeftChecked = _lastLeftChecked;
                IsRightChecked = _lastRightChecked;
            }
        }
        else
        {
            // 해제 직전 상태 기억
            _lastLeftChecked = IsLeftChecked;
            _lastRightChecked = IsRightChecked;

            // 둘 다 OFF
            IsLeftChecked = false;
            IsRightChecked = false;
        }
        RaiseValueChanged();
    }

    /// <summary>
    /// 라디오 선택시 마지막 상태 기억
    /// </summary>
    private void RememberRadio(bool left)
    {
        _lastLeftChecked = left;
        _lastRightChecked = !left;
    }

    /// <summary>
    /// 내부 값 변경 시 외부에 이벤트 알림
    /// </summary>
    private void RaiseValueChanged()
    {
        ValueChanged?.Invoke(this, new RoutedEventArgs());
    }

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(VsCheckSelector), new PropertyMetadata(false));
    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly DependencyProperty IsLeftCheckedProperty =
        DependencyProperty.Register(nameof(IsLeftChecked), typeof(bool), typeof(VsCheckSelector), new PropertyMetadata(false));
    public bool IsLeftChecked
    {
        get => (bool)GetValue(IsLeftCheckedProperty);
        set => SetValue(IsLeftCheckedProperty, value);
    }

    public static readonly DependencyProperty IsRightCheckedProperty =
        DependencyProperty.Register(nameof(IsRightChecked), typeof(bool), typeof(VsCheckSelector), new PropertyMetadata(false));
    public bool IsRightChecked
    {
        get => (bool)GetValue(IsRightCheckedProperty);
        set => SetValue(IsRightCheckedProperty, value);
    }

    public static readonly DependencyProperty CheckTextProperty =
        DependencyProperty.Register(nameof(CheckText), typeof(string), typeof(VsCheckSelector), new PropertyMetadata("체크"));
    public string CheckText
    {
        get => (string)GetValue(CheckTextProperty);
        set => SetValue(CheckTextProperty, value);
    }

    public static readonly DependencyProperty LeftRadioTextProperty =
        DependencyProperty.Register(nameof(LeftRadioText), typeof(string), typeof(VsCheckSelector), new PropertyMetadata("LEFT"));
    public string LeftRadioText
    {
        get => (string)GetValue(LeftRadioTextProperty);
        set => SetValue(LeftRadioTextProperty, value);
    }

    public static readonly DependencyProperty RightRadioTextProperty =
        DependencyProperty.Register(nameof(RightRadioText), typeof(string), typeof(VsCheckSelector), new PropertyMetadata("RIGHT"));
    public string RightRadioText
    {
        get => (string)GetValue(RightRadioTextProperty);
        set => SetValue(RightRadioTextProperty, value);
    }

    public static readonly DependencyProperty GroupNameProperty =
        DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(VsCheckSelector), new PropertyMetadata("AxisSelectorGroup"));
    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

    /// <summary>
    /// 각 라인별 축 최대값을 위한 바인딩 프로퍼티입니다.
    /// </summary>
    public static readonly DependencyProperty AxisMaxProperty =
        DependencyProperty.Register(nameof(AxisMax), typeof(string), typeof(VsCheckSelector), new PropertyMetadata("20"));

    /// <summary>
    /// 축 최대값 (텍스트박스와 바인딩)
    /// </summary>
    public string AxisMax
    {
        get => (string)GetValue(AxisMaxProperty);
        set => SetValue(AxisMaxProperty, value);
    }  
}
