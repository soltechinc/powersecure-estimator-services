// Any parameters needs to be true, if not the value is false
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives {
    public class OrPrimitive : IPrimitive {
        public string Name => "or";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository) {
            return parameters.ToBoolean().Any(p => p);
        }

        public (bool Success, string Message) Validate(JToken jToken) {
            if(jToken.Children().Count() < 1) {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");            
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array)) {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
