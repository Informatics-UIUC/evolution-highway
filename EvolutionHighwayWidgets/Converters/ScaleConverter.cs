using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayWidgets.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = (double)value;
            return dValue / 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = (double)value;
            return dValue * 1;
        }
    }
}
