using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace powersecure_instruction_set_engine.Primitives
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
                    IPrimitive primitive = (IPrimitive)Activator.CreateInstance(type);
                    primitives.Add(primitive.Name.ToLower(), primitive);
                });

            return primitives.ToReadonlyDictionary();
        }
    }
}
