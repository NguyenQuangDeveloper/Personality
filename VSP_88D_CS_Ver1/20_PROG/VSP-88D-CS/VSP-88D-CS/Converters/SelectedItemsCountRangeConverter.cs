using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace VSP_88D_CS.Converters;

public class SelectedItemsCountRangeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var count = value is ICollection collection ? collection.Count : 0;

        if (parameter is string paramString && !string.IsNullOrEmpty(paramString))
        {
            bool bCheck = false;

            var parts = paramString.Split('-');
            if (int.TryParse(parts[0], out int min))
            {
                bCheck = count >= min;
            }
            if (parts.Length == 2 && int.TryParse(parts[1], out int max))
            {
                bCheck = bCheck && count <= max;
            }

            return bCheck;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
