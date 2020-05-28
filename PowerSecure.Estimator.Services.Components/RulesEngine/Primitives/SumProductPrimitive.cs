// 2 parameters. Both parameters are equal-sized arrays of numerics.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SumProductPrimitive : IFunction
    {
        public string Name => "sumproduct";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var values = parameters[0].ToObjectArray().ToDecimal().ToArray();
            var factors = parameters[1].ToObjectArray().ToDecimal().ToArray();

            decimal value = 0;

            int length = values.Length > factors.Length ? factors.Length : values.Length;

            for (int i = 0; i < length; ++i)
            {
                value += (values[i] ?? 0) * (factors[i] ?? 0);
            }

            return value;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2)
            {
                return (false, $"Expected a parameter array of length 2, got the following: {jToken.Children().Count()}");
            }

            if (!jToken.Children().All(p => p.Type == JTokenType.Array))
            {
                return (false, "Expected all parameters to be arrays.");
            }

            var list = new List<JToken>(jToken.Children());
            if(list[0].Children().Count() == 0 || list[1].Children().Count() == 0)
            {
                return (false, "Expected parameter arrays to have entries.");
            }

            return (true, string.Empty);
        }
    }
}
