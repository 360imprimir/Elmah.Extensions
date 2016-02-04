using System;
using System.Collections;

namespace Elmah.Extensions
{
    static class DictionaryExtensions
    {
        public static T Find<T>(this IDictionary dict, object key, T @default)
        {
            if (dict == null) throw new ArgumentNullException("dict");
            return (T)(dict[key] ?? @default);
        }
    }
}