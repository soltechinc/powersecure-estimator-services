// 1 or more parameters.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SubtractionPrimitive : IPrimitive
    {
        public string Name => "-";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = parameters.ToDecimal().ToArray();

            if(decimals.Length == 1)
            {
                return -decimals[0];
            }

            decimal value = decimal.MinValue;
            decimals.ForEach(p => {
                if (value == decimal.MinValue)
                {
                    value = p;
                }
                else
                {
                    value -= p;
                }
            });
            return value;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
