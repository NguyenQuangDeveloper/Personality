using System.Windows;
using System.Windows.Media;
using VSLibrary.UIComponent.VSControls;

namespace VSP_88D_CS.Common.Controls;

/// <summary>
/// VS Button for using one distributed color. Tested and applied in XAML but not successfully.
/// </summary>
public class UnifiedColorVsButton : VsButton
{
    public static readonly DependencyProperty UnifiedColorProperty =
        DependencyProperty.Register(nameof(UnifiedColor), typeof(Brush), typeof(UnifiedColorVsButton),
            new PropertyMetadata(null, OnUnifiedColorChanged));

    public Brush UnifiedColor
    {
        get => (Brush)GetValue(UnifiedColorProperty);
        set => SetValue(UnifiedColorProperty, value);
    }

    private static void OnUnifiedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UnifiedColorVsButton button && e.NewValue is Brush brush)
        {
            button.Background = brush;
            button.BackgroundTop = brush;
            button.ShineColor = brush;
            button.ShineColorBottom = brush;
        }
    }

    public UnifiedColorVsButton()
    {
        Foreground = Brushes.Blue;
    }
}
