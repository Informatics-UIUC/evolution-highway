using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class OrientationToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orientation = (Orientation) value;

            switch (orientation)
            {
                case Orientation.Vertical:
                    return 0d;

                case Orientation.Horizontal:
                    return 270d;
            }

            throw new ArgumentOutOfRangeException("value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
