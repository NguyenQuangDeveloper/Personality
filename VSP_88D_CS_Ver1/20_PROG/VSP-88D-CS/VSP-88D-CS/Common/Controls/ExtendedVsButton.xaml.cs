using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VSP_88D_CS.Common.Controls;

/// <summary>
/// Interaction logic for UserBasedVsButtonControl.xaml
/// </summary>
public partial class ExtendedVsButton : UserControl
{
    public ExtendedVsButton()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ImageSourceProperty =
    DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ExtendedVsButton), new PropertyMetadata(null));

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExtendedVsButton), new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ExtendedVsButton), new PropertyMetadata(null));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty ButtonHeightProperty =
        DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(ExtendedVsButton), new PropertyMetadata(50.0));

    public double ButtonHeight
    {
        get => (double)GetValue(ButtonHeightProperty);
        set => SetValue(ButtonHeightProperty, value);
    }

    public static readonly DependencyProperty ButtonForegroundProperty =
        DependencyProperty.Register(nameof(ButtonForeground), typeof(Brush), typeof(ExtendedVsButton), new PropertyMetadata(Brushes.Blue));

    public Brush ButtonForeground
    {
        get => (Brush)GetValue(ButtonForegroundProperty);
        set => SetValue(ButtonForegroundProperty, value);
    }

    public static readonly DependencyProperty CommandParameterProperty =
    DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ExtendedVsButton), new PropertyMetadata(null));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

}
