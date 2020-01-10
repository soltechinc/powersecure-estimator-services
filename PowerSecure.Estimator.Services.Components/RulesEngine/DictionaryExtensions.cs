using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace powersecure_instruction_set_engine
{
    public static class DictionaryExtensions
    {
        public static ReadOnlyDictionary<TKey, TValue> ToReadonlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }
    }
}