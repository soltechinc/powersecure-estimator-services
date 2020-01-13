using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class DivisionPrimitive : IPrimitive
    {
        public string Name => "/";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = Primitive.ConvertToDecimal(parameters);

            decimal value = decimal.MinValue;
            decimals.ForEach(p => {
                if (value == decimal.MinValue)
                {
                    value = p;
                }
                else
                {
                    value /= p;
                }
            });
            return value;
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 2)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 2 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
