using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
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
    }
}
