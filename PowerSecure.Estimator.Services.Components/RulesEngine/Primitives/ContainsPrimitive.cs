using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class ContainsPrimitive : IFunction
    {
        public string Name => "contains";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            string[] strings = parameters.ToObjectArray().ToRawString().ToArray();
            return strings[0].Contains(strings[1]);
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2)
            {
                return (false, $"Expected a parameter array of length 2, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
