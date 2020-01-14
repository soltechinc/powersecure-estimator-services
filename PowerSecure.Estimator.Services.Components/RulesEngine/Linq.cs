using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<decimal> ToDecimal(this IEnumerable<object> objects)
        {
            foreach(var obj in objects)
            {
                switch (obj)
                {
                    case string s:
                        {
                            yield return decimal.Parse(s);
                            break;
                        }
                    default:
                        {
                            yield return Convert.ToDecimal(obj);
                            break;
                        }
                }
            }
        }
    }
}
