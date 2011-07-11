using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class NumberToPrettyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) parameter = 0;

            var formatString = String.Format("{{0:N{0}}}", parameter);
            return String.Format(formatString, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
