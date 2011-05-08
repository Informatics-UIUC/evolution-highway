using System;
using System.Collections.Generic;

namespace EvolutionHighwayApp.Utils
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
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
    }
}
