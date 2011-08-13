using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EvolutionHighwayApp.Converters
{
    [ContentProperty("Values")]
    public class SignConverter : IValueConverter
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(Collection<DependencyObject>), typeof(SignConverter), null);

        public Collection<DependencyObject> Values { get; set; }

        #region Sign attached property definition
        public static readonly DependencyProperty SignProperty =
            DependencyProperty.RegisterAttached("Sign", typeof(int?), typeof(SignConverter), null);

        public static void SetSign(DependencyObject obj, int? sign)
        {
            obj.SetValue(SignProperty, sign);
        }

        public static int? GetSign(DependencyObject obj)
        {
            return (int?)obj.GetValue(SignProperty);
        }
        #endregion

        public SignConverter()
        {
            Values = new Collection<DependencyObject>();
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o != null && o.GetType() != typeof(int))
                throw new ArgumentException("Expected integer type for conversion");

            var sign = (int?)o;
            var convertedValue = (from DependencyObject depObj in Values
                                  let depObjSign = GetSign(depObj)
                                  where sign == depObjSign
                                  select depObj).FirstOrDefault() ?? (from DependencyObject depObj in Values
                                                                      let depObjSign = GetSign(depObj)
                                                                      where depObjSign == null
                                                                      select depObj).FirstOrDefault();

            if (!targetType.IsAssignableFrom(convertedValue.GetType()))
                throw new InvalidCastException(
                    String.Format("Cannot cast {0} into {1} - Values must be of type {1}", convertedValue.GetType(), targetType));

            return convertedValue;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
