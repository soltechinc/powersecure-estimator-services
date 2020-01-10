using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class DictionaryExtensions
    {
        public static ReadOnlyDictionary<TKey, TValue> ToReadonlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }
    }
}