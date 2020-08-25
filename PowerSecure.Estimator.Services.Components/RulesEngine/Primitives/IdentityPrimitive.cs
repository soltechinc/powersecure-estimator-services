using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class IdentityPrimitive : IFunction
    {
        public string Name => "identity";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters[0].ToResolvedParameter();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 1)
            {
                return (false, $"Expected a parameter array of length 1, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
