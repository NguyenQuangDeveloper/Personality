using System.Globalization;
using System.Windows.Data;

namespace VSLibrary.UIComponent.Converters;

/// <summary>
/// Boolean 값을 반전시키거나 long 값의 0을 true로, 그 외를 false로 변환하는 컨버터입니다.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// Boolean 또는 long 값을 반전시키는 메서드입니다.
    /// </summary>
    /// <param name="value">변환할 값입니다. bool 또는 long 타입을 허용합니다.</param>
    /// <param name="targetType">대상 타입입니다. 사용되지 않습니다.</param>
    /// <param name="parameter">추가 파라미터입니다. 사용되지 않습니다.</param>
    /// <param name="culture">문화권 정보입니다. 사용되지 않습니다.</param>
    /// <returns>bool 값을 반전시키거나 long 값이 0이면 true, 그 외는 false를 반환합니다.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long intValue)
        {
            // long 값이 0이면 true, 그 외는 false로 변환
            return intValue == 0 ? true : false;
        }
        else if (value is bool booleanValue)
        {
            // Boolean 값을 반전시킴
            return !booleanValue;
        }

        // 기본값은 false를 반환하여 예외 상황 방지
        return false;
    }

    /// <summary>
    /// Boolean 값을 long 또는 int 값으로, 반대로 변환하는 메서드입니다.
    /// </summary>
    /// <param name="value">변환할 값입니다. bool 타입을 허용합니다.</param>
    /// <param name="targetType">대상 타입입니다. long 또는 int로 변환할 수 있습니다.</param>
    /// <param name="parameter">추가 파라미터입니다. 사용되지 않습니다.</param>
    /// <param name="culture">문화권 정보입니다. 사용되지 않습니다.</param>
    /// <returns>Boolean 값을 반전시켜 long이나 int 값으로 변환하거나 반전된 Boolean 값을 반환합니다.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            // Boolean 값을 반전시킴
            return !booleanValue;
        }
        else if (targetType == typeof(long) || targetType == typeof(int))
        {
            // Boolean 값을 long이나 int 값으로 변환
            return (bool)value ? 0L : 1L;  // 반전된 값을 long 또는 int로 반환
        }

        // 기본값은 0을 반환하여 예외 상황 방지
        return 0L;
    }
}
