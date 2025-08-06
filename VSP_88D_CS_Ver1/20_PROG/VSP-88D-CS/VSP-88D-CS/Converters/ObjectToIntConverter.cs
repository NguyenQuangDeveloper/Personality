using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VSP_88D_CS.Converters
{
    public class ObjectToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // object → int
            if (value == null) return 0;

            if (value is int i)
                return i;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return 0; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // int → object
            return value;
        }
    }
}
