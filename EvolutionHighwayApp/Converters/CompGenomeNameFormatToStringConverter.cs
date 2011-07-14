using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace EvolutionHighwayApp.Converters
{
    public class CompGenomeNameFormatToStringConverter : IValueConverter
    {
        public static int NameFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parts = value.ToString().Split(':');
            var result = new List<string>();

            if ((NameFormat & (int)CompGenomeNameFormat.Genome) > 0) result.Add(parts[0]);
            if (parts.Length > 1 && (NameFormat & (int)CompGenomeNameFormat.Species) > 0) result.Add(parts[1]);
            if (parts.Length > 2 && (NameFormat & (int)CompGenomeNameFormat.Custom) > 0) result.Add(parts[2]);

            return result.Aggregate((s, e) => s + ":" + e).Trim(':');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
