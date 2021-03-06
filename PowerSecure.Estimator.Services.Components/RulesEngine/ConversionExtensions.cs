﻿using System;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Conversions
{
    public static class ConversionExtensions
    {
        public static decimal? ToDecimal(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToDecimal();
                    }
                case decimal d:
                    {
                        return d;
                    }
                case string s when !s.StartsWith('$'):
                    {
                        return decimal.Parse(s);
                    }
                case string s when s.StartsWith('$'):
                    {
                        return decimal.Parse(s.Substring(1));
                    }
                case bool b:
                    {
                        return b ? 1m : 0m;
                    }
                default:
                    {
                        return Convert.ToDecimal(obj);
                    }
            }
        }

        public static bool? ToBoolean(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToBoolean();
                    }
                case bool b:
                    {
                        return b;
                    }
                case decimal d:
                    {
                        return d != 0m;
                    }
                case string s when !s.StartsWith('$'):
                    {
                        return bool.Parse(s);
                    }
                case string s when s.StartsWith('$'):
                    {
                        return bool.Parse(s.Substring(1));
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
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToRawString();
                    }
                case string s when s.StartsWith('$'):
                    {
                        return s.Substring(1);
                    }
                case object[] o:
                    {
                        return string.Empty;
                    }
                default:
                    {
                        return obj.ToString();
                    }
            }
        }

        public static string ToStringLiteral(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToStringLiteral();
                    }
                case string s when s.StartsWith('$'):
                    {
                        return s;
                    }
                case object[] o:
                    {
                        return "$";
                    }
                default:
                    {
                        return "$" + obj.ToString();
                    }
            }
        }

        public static object[] ToObjectArray(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToObjectArray();
                    }
                default:
                    {
                        return (object[])obj;
                    }
            }
        }

        public static IComparable ToComparable(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve()?.ToComparable();
                    }
                case decimal d:
                    {
                        return d;
                    }
                case string s:
                    {
                        if (s.StartsWith('$'))
                        {
                            return s.Substring(1).ToLower();
                        }

                        if (decimal.TryParse(s, out decimal d))
                        {
                            return d;
                        }

                        if (bool.TryParse(s, out bool b))
                        {
                            return b ? 1m : 0m;
                        }

                        return s.ToLower();
                    }
                case bool b:
                    {
                        return b ? 1m : 0m;
                    }
                default:
                    {
                        return (IComparable)obj;
                    }
            }
        }

        public static object ToResolvedParameter(this object obj)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve();
                    }
                default:
                    return obj;
            }
        }

        public static object ToInstructionSet(this object obj, object p, string searchString)
        {
            switch (obj)
            {
                case null:
                    {
                        return null;
                    }
                case UnresolvedParameter parameter:
                    {
                        return parameter.ToInstructionSet(p, searchString);
                    }
                default:
                    return obj;
            }
        }

        public static IEnumerable<decimal?> ToDecimal(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToDecimal();
            }
        }

        public static IEnumerable<bool?> ToBoolean(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToBoolean();
            }
        }

        public static IEnumerable<IComparable> ToComparable(this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                yield return obj.ToComparable();
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
