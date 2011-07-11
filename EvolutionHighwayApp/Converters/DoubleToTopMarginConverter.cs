using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class DoubleToTopMarginConverter : IValueConverter
    {
        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("0,{0},0,0", ScaleConverter.Convert(value, null, null, null));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
