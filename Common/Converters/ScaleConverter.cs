using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayWidgets.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public static double Scale { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = (double)value;
            return dValue / Scale;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = (double)value;
            return dValue * Scale;
        }
    }
}
