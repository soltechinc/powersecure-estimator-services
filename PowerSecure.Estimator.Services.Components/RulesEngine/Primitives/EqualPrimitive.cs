// 4 parameters. If the first two parameters are equal, returns parameter 3. Otherwise, returns parameter 4.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class EqualPrimitive : IPrimitive
    {
        public string Name => "=";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters[0].ToComparable().CompareTo(parameters[1].ToComparable()) == 0 ? parameters[2] : parameters[3];
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
