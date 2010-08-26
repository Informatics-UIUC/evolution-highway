using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayWidgets.Converters
{
    public class TopMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("0,{0},0,0", new ScaleConverter().Convert(value, null, null, null));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
