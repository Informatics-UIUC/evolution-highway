using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class CompGenomeNameFormatToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = ParseCompGenomeNameFormat(parameter);
            return (((int) value) & ((int) format)) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) ParseCompGenomeNameFormat(parameter);
        }

        private static CompGenomeNameFormat ParseCompGenomeNameFormat(object parameter)
        {
            CompGenomeNameFormat format;
            if (!Enum.TryParse(parameter.ToString(), true, out format))
                throw new ArgumentException("Invalid converter parameter: " + parameter, "parameter");

            return format;
        }
    }
}
