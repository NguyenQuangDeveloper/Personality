using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VSLibrary.UIComponent.Converters;

/// <summary>
/// Int64 (long) 값을 Int32 (int) 값으로 변환하는 컨버터입니다.
/// </summary>
public class Int64ToInt32Converter : IValueConverter
{
    /// <summary>
    /// Int64 값을 Int32 값으로 변환하는 메서드입니다.
    /// </summary>
    /// <param name="value">변환할 값입니다. long 값이어야 합니다.</param>
    /// <param name="targetType">대상 타입입니다. 사용되지 않습니다.</param>
    /// <param name="parameter">추가 파라미터입니다. 사용되지 않습니다.</param>
    /// <param name="culture">문화권 정보입니다. 사용되지 않습니다.</param>
    /// <returns>Int32 값으로 변환된 값입니다. long 값이 아닌 경우는 그대로 반환합니다.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long longValue)
        {
            return (int)longValue;  // long 값을 int로 변환
        }
        else if (value is int intValue)
        {
            int rtn = intValue < 0 ? 0 : intValue;
            return rtn;  // 
        }
        return value;  // long 값이 아닌 경우는 그대로 반환
    }

    /// <summary>
    /// Int32 값을 다시 Int64 값으로 변환하는 메서드입니다. 
    /// 이 컨버터에서는 특별한 변환 없이 값을 그대로 반환합니다.
    /// </summary>
    /// <param name="value">변환할 값입니다.</param>
    /// <param name="targetType">대상 타입입니다.</param>
    /// <param name="parameter">추가 파라미터입니다.</param>
    /// <param name="culture">문화권 정보입니다.</param>
    /// <returns>변환되지 않은 원래의 값을 반환합니다.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;  // 특별한 변환 없이 원래 값을 반환
    }
}