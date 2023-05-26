using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDependencyInjection
{
    internal static class Utils
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> factory)
            where TKey : notnull
        {
            if (dict.TryGetValue(key, out TValue? value)) 
                return value;

            TValue newValue = factory.Invoke();
            dict.Add(key, newValue);
            return newValue;
        }
    }
}
