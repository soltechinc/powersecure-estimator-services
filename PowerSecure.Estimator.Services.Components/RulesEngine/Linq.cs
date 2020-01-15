using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action = null)
        {
            foreach (T item in source)
            {
                action?.Invoke(item);
            }
        }

        public static decimal ToDecimal(this object obj)
        {
            switch (obj)
            {
                case decimal d:
                    {
                        return d;
                    }
                case string s:
                    {
                        return decimal.Parse(s);
                    }
                default:
                    {
                        return Convert.ToDecimal(obj);
                    }
            }
        }

        public static IEnumerable<decimal> ToDecimal(this IEnumerable<object> objects)
        {
            foreach(var obj in objects)
            {
                yield return obj.ToDecimal();
            }
        }

        public static bool ToBoolean(this object obj)
        {
            switch (obj)
            {
                case bool b:
                    {
                        return b;
                    }
                case decimal d:
                    {
                        return d != 0m;
                    }
                default:
                    {
                        return Convert.ToBoolean(obj);
                    }
            }
        }

        public static string ToRawString(this object obj)
        {
            switch (obj)
            {
                case string s when s.StartsWith('$'):
                    {
                        return s.Substring(1);
                    }
                default:
                    {
                        return obj.ToString();
                    }
            }
        }

        public static IEnumerable<string> ToRawString(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToRawString();
            }
        }

        public static string ToStringLiteral(this object obj)
        {
            switch (obj)
            {
                case string s when s.StartsWith('$'):
                    {
                        return s;
                    }
                default:
                    {
                        return "$" + obj.ToString();
                    }
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
