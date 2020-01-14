// 1 or more parameters.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class MaxPrimitive : IPrimitive
    {
        public string Name => "max";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters.ToDecimal().Max();
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
