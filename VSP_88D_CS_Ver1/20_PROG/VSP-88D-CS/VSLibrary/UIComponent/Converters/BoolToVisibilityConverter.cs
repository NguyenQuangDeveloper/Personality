using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VSLibrary.UIComponent.Converters;

/// <summary>
/// bool → Visibility 변환 컨버터
/// true = Visible, false = Collapsed
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 변환 메서드 (bool → Visibility)
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// 역변환은 지원 안함
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
