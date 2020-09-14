using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SubStringPrimitive : IFunction
    {
        public string Name => "substr";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            string s = parameters[0].ToRawString();
            int index = (int)parameters[1].ToDecimal();
            int length = parameters.Length == 2 ? -1 : (int)parameters[2].ToDecimal();

            return parameters.Length == 2 ? s.Substring(index) : s.Substring(index, (int)parameters[2].ToDecimal());
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2 || jToken.Children().Count() != 3)
            {
                return (false, $"Expected a parameter array of length 2 or 3, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
