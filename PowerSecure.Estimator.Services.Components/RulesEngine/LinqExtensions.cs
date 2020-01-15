using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action = null)
        {
            foreach (T item in source)
            {
                action?.Invoke(item);
            }
        }

        public static IEnumerable<decimal> ToDecimal(this IEnumerable<object> objects)
        {
            foreach(var obj in objects)
            {
                yield return obj.ToDecimal();
            }
        }

        public static IEnumerable<string> ToRawString(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToRawString();
            }
        }

        public static IEnumerable<string> ToStringLiteral(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToStringLiteral();
            }
        }
    }
}
