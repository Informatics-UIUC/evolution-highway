using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media;
using System.Globalization;

namespace EvolutionHighwayApp.Utils
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValueProvider();
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue val)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = val;
            else
                dictionary.Add(key, val);
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator) where T : class
        {
            var array = source.Select(obj => (obj == null) ? "<null>" : obj.ToString()).ToArray();

            return string.Join(separator, array);
        }

        public static string ToString(this IEnumerable source, string separator)
        {
            var array = source.Cast<object>().Select(obj => (obj == null) ? "<null>" : obj.ToString()).ToArray();

            return string.Join(separator, array);
        }

        public static bool InDesignMode(this FrameworkElement element)
        {
            return (Application.Current == null) || Application.Current.GetType() == typeof(Application);
        }

        public static IEnumerable<Enum> GetValues(this Enum @enum)
        {
            var values = new List<Enum>();
            var fields = from field in @enum.GetType().GetFields()
                        where field.IsLiteral && field.IsStatic
                        select field;
            values.AddRange(fields.Select(f => (Enum)f.GetValue(@enum)));

            return values;
        }

        public static Color FromHexString(this Color color, string hexColor)
        {
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            if (hexColor.Length != 8 && hexColor.Length != 6)
                throw new ArgumentException("The color must be in one of the following (case-insensitive) formats: #AARRGGBB or #RRGGBB");

            var a = (byte) 255;
            if (hexColor.Length == 8)
            {
                a = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                hexColor = hexColor.Substring(2);
            }

            var r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        public static void LogError(this object obj)
        {
            var window = HtmlPage.Window;
            var isErrorLogAvailable = (bool)window.Eval("typeof(debug) != 'undefined' && typeof(debug.error) != 'undefined'");
            if (!isErrorLogAvailable) return;

            var errorLog = (window.Eval("debug.error") as ScriptObject);
            if (errorLog != null)
                errorLog.InvokeSelf(obj.ToString());
        }
    }
}
