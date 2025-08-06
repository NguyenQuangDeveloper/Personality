using System.Globalization;
using System.Windows.Data;

namespace VSP_88D_CS.Converters
{
    public class DoubleToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (double.TryParse(parameter.ToString(), out double targetValue) &&
                double.TryParse(value.ToString(), out double currentValue))
            {
                return Math.Abs(currentValue - targetValue) < 0.0001;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value && double.TryParse(parameter.ToString(), out double result))
            {
                return result;
            }

            return Binding.DoNothing;
        }
    }

}
