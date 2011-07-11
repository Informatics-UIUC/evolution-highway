using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public static double DisplayMaximum { get; set; }
        public static double DataMaximum { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = (double) value;
            return dValue * DisplayMaximum / DataMaximum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
