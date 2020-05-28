using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class IsTruePrimitive : IFunction
    {
        public string Name => "istrue";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return (parameters[0] != null && !string.IsNullOrEmpty(parameters[0].ToRawString()) && parameters[0].ToBoolean().Value);
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 1)
            {
                return (false, $"Expected a parameter array of length 1, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
