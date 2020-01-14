using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class MultiplicationPrimitive : IPrimitive
    {
        public string Name => "*";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            decimal value = 1;
            parameters.ToDecimal().ForEach(p => value *= p);
            return value;
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 2)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 2 or more, got the following: {jToken.Children().Count()}");
            }

            if(jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
