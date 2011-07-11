using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
    }
}
