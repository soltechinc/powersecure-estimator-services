using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public static class Primitive
    {
        public delegate Decimal ParamsFunc(params object[] parameters);

        public static IDictionary<string, IPrimitive> LoadFromAssembly()
        {
            var primitives = new Dictionary<string, IPrimitive>();

            Assembly.GetAssembly(typeof(Primitive)).GetTypes()
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
                string s = obj as string;
                if(s != null)
                {
                    list.Add(decimal.Parse(s));
                    continue;
                }

                list.Add(Convert.ToDecimal(obj));
            }

            return list.ToArray();
        }
    }
}
