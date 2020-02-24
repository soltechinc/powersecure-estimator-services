// 3 parameters. Parameter 1 is a boolean, parameter 2 is the value returned if true, parameter 3 is the value returned if false.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class CheckPrimitive : IFunction
    {
        public string Name => "if";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters[0].ToBoolean() ? parameters[1] : parameters[2];
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 3)
            {
                return (false, $"Expected a parameter array of length 3, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
