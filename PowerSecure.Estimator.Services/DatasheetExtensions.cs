using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services
{
    public static class DatasheetExtensions
    {
        public static void Apply(this IDictionary<string,object> dict, Func<(string,object), object> func)
        {
            foreach(var pair in new Dictionary<string,object>(dict))
            {
                switch(pair.Value)
                {
                    case IDictionary<string, object> innerDict:
                        {
                            Apply(innerDict, func);
                        }
                        break;
                    case IEnumerable<object> list:
                        {
                            foreach (var element in list)
                            {
                                switch (element)
                                {
                                    case IDictionary<string, object> innerDict:
                                        {
                                            Apply(innerDict, func);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        {
                            dict[pair.Key] = func((pair.Key, pair.Value));
                        }
                        break;
                }
            }
        }
    }
}
