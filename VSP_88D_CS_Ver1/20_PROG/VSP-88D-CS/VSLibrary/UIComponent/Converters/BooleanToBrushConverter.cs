using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VSLibrary.UIComponent.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public Brush OnBrush { get; set; } = Brushes.Lime;
    public Brush OffBrush { get; set; } = Brushes.Gray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = false;
        if (value is bool b)
            flag = b;
        return flag ? OnBrush : OffBrush;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
