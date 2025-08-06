using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VSP_88D_CS.Converters
{
    public class RadioButtonConverter : IValueConverter
    {
        /// <summary>
        /// ViewModel의 SelectedRange 값과 RadioButton의 ConverterParameter를 비교하여
        /// IsChecked 값을 반환합니다.
        /// </summary>
        /// <param name="value">ViewModel의 SelectedRange 값</param>
        /// <param name="targetType">타겟 타입</param>
        /// <param name="parameter">RadioButton의 ConverterParameter 값</param>
        /// <param name="culture">문화권 정보</param>
        /// <returns>RadioButton이 선택되었는지 여부</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        /// <summary>
        /// RadioButton의 IsChecked 값이 변경되었을 때 ViewModel의 SelectedRange 값을 설정합니다.
        /// </summary>
        /// <param name="value">RadioButton의 IsChecked 값</param>
        /// <param name="targetType">타겟 타입</param>
        /// <param name="parameter">RadioButton의 ConverterParameter 값</param>
        /// <param name="culture">문화권 정보</param>
        /// <returns>선택된 RadioButton의 ConverterParameter 값 또는 null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                return parameter?.ToString();
            }
            return null;
        }
    }
}
