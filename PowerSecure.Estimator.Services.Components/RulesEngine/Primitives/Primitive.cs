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
        public static IDictionary<string, IFunction> Load(Assembly assembly)
        {
            var functions = new Dictionary<string, IFunction>();

            assembly.GetTypes()
                .Where(p => typeof(IFunction).IsAssignableFrom(p) && p.IsClass)
                .Aggregate(functions, (acc,type) =>
                {
                    var primitive = (IFunction)Activator.CreateInstance(type);
                    acc.Add(primitive.Name.ToLower(), primitive);
                    return acc;
                });

            return functions;
        }
    }
}
