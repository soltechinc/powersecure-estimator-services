// 2 or more parameters, or 1 parameter if it is an array.
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
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters.Length > 1 ?
                parameters.ToDecimal().Aggregate((decimal?)null, (acc, current) => acc == null ? current : acc / current) :
                parameters[0].ToObjectArray().ToDecimal().Aggregate((decimal?)null, (acc, current) => acc == null ? current : acc / current);
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                if (jToken.Children().Count() > 1)
                {
                    return (false, "Did not expect any arrays as parameters.");
                }

                var child = jToken.Children().First();
                if (child.Children().Count() < 2)
                {
                    return (false, $"Expected an array of length 2 or more as a parameter, got the following: {child.Children().Count()}");
                }

                if (child.Children().Any(p => p.Type == JTokenType.Array))
                {
                    return (false, "Did not expect any nested arrays as parameters.");
                }
            }
            else
            {
                if (jToken.Children().Count() < 2)
                {
                    return (false, $"Expected a parameter array of length 2 or more, got the following: {jToken.Children().Count()}");
                }
            }

            return (true, string.Empty);
        }
    }
}
