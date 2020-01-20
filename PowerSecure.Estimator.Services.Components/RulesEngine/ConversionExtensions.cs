using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class ConversionExtensions
    {
        public static decimal ToDecimal(this object obj)
        {
            switch (obj)
            {
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToDecimal();
                    }
                case decimal d:
                    {
                        return d;
                    }
                case string s:
                    {
                        return decimal.Parse(s);
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

        public static bool ToBoolean(this object obj)
        {
            switch (obj)
            {
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToBoolean();
                    }
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
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToRawString();
                    }
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

        public static string ToStringLiteral(this object obj)
        {
            switch (obj)
            {
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToStringLiteral();
                    }
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

        public static object[] ToObjectArray(this object obj)
        {
            switch (obj)
            {
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToObjectArray();
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
                case UnresolvedParameter parameter:
                    {
                        return parameter.Resolve().ToComparable();
                    }
                default:
                    {
                        return (IComparable)obj;
                    }
            }
        }
    }
}
