using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class BoolToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orientation = (Orientation) value;

            Orientation param;
            if (!Enum.TryParse(parameter.ToString(), out param))
                throw new ArgumentException("Could not parse the parameter value as Orientation", "parameter");

            return orientation == param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChecked = (bool) value;

            Orientation param;
            if (!Enum.TryParse(parameter.ToString(), out param))
                throw new ArgumentException("Could not parse the parameter value as Orientation", "parameter");
            
            switch (param)
            {
                case Orientation.Horizontal:
                    return isChecked ? param : Orientation.Vertical;

                case Orientation.Vertical:
                    return isChecked ? param : Orientation.Horizontal;
            }

            throw new ArgumentOutOfRangeException("parameter");
        }
    }
}
