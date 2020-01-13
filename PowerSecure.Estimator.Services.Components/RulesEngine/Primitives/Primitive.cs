using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public static partial class Primitive
    {
        public delegate Decimal ParamsFunc(params object[] parameters);

        public static IDictionary<string, IPrimitive> LoadFromAssembly(Assembly assembly)
        {
            var primitives = new Dictionary<string, IPrimitive>();

            assembly.GetTypes()
                .Where(p => typeof(IPrimitive).IsAssignableFrom(p) && p.IsClass)
                .ForEach(type =>
                {
                    var primitive = (IPrimitive)Activator.CreateInstance(type);
                    primitives.Add(primitive.Name.ToLower(), primitive);
                });

            return primitives.ToReadonlyDictionary();
        }

        public static decimal[] ConvertToDecimal(object[] objects)
        {
            var list = new List<decimal>();

            foreach(var obj in objects)
            {
                switch(obj)
                {
                    case string s:
                        {
                            list.Add(decimal.Parse(s));
                            break;
                        }
                    default:
                        {
                            list.Add(Convert.ToDecimal(obj));
                            break;
                        }
                }
            }

            return list.ToArray();
        }
    }
}
