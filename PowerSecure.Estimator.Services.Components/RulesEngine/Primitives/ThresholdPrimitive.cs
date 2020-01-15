// 4 parameters. If parameter 1 is greater than or equal to parameter 2, parameter 3 is returned. Otherwise, parameter 4 is returned.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class ThresholdPrimitive : IPrimitive
    {
        public string Name => "threshold";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters[0].ToDecimal() >= parameters[1].ToDecimal() ? parameters[2] : parameters[3];
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 4)
            {
                return (false, $"Expected a parameter array of length 4, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
