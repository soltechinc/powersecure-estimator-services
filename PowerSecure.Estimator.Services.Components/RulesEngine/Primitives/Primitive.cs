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
        public static IDictionary<string, IPrimitive> Load(Assembly assembly)
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

        public static decimal[] ConvertToDecimal(params object[] objects)
        {
            return new List<decimal>(objects.ToDecimal()).ToArray();
        }
    }
}
