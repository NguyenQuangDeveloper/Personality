using System.Globalization;
using System.Windows.Data;

namespace VSP_88D_CS.Converters;

public class BothFalseMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 &&
            values[0] is bool createMode &&
            values[1] is bool editMode)
        {
            return !createMode && !editMode;
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
