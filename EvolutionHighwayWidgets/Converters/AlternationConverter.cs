using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EvolutionHighwayWidgets.Converters
{
    [ContentProperty("Values")]
    public class AlternationConverter: IValueConverter
    {
        public static readonly DependencyProperty ValuesProperty =
           DependencyProperty.Register("Values", typeof(Collection<object>), typeof(AlternationConverter), null);

        public Collection<object> Values { get; set; }


        public AlternationConverter()
        {
            Values = new Collection<object>();
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o.GetType() != typeof(int))
                throw new ArgumentException("Expected integer type for conversion");
            
            var index = (int) o;
            if (index < 0 || index > Values.Count-1)
                throw new ArgumentOutOfRangeException("Not enough values defined for the alternation index received", (Exception)null);

            var value = Values[index];

            if (!targetType.IsAssignableFrom(value.GetType()))
                throw new InvalidCastException(
                    String.Format("Cannot cast {0} into {1} - Values must be of type {1}", value.GetType(), targetType));

            return value;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
