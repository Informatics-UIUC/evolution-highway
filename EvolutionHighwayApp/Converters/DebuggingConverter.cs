using System;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class DebuggingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value; // Add the breakpoint here!!
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
