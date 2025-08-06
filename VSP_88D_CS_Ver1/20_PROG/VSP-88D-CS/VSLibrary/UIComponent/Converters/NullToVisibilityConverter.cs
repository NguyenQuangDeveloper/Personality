using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VSLibrary.UIComponent.Converters
{
    /// <summary>
    /// 값이 null인지 여부에 따라 Visibility를 반환하는 컨버터입니다.
    /// null이면 Collapsed, null이 아니면 Visible을 반환합니다.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// true일 경우, 결과를 반대로 반환합니다. (null → Visible, not null → Collapsed)
        /// </summary>
        public bool Inverse { get; set; } = false;

        /// <summary>
        /// 값의 null 여부를 기반으로 Visibility 값을 반환합니다.
        /// </summary>
        /// <param name="value">바인딩된 값</param>
        /// <param name="targetType">타겟 타입 (무시됨)</param>
        /// <param name="parameter">추가 파라미터 (무시됨)</param>
        /// <param name="culture">문화권 정보</param>
        /// <returns>Visible 또는 Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = value != null;
            if (Inverse)
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// ConvertBack은 지원하지 않습니다.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("NullToVisibilityConverter는 ConvertBack을 지원하지 않습니다.");
        }
    }
}
