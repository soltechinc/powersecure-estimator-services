// 1 or more parameters.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class NotPrimitive : IPrimitive
    {
        public string Name => "not";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            if (parameters.Length > 1)
            {
                return parameters.ToBoolean().Select(b => (object)!b).ToArray();
            }

            switch (parameters[0].ToResolvedParameter())
            {
                case object[] objs:
                    {
                        return objs.ToBoolean().Select(b => (object)!b).ToArray();
                    }
                default:
                    {
                        return !parameters[0].ToBoolean();
                    }
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                if (jToken.Children().Count() > 1)
                {
                    return (false, "Did not expect any arrays as parameters.");
                }

                var child = jToken.Children().First();
                if (child.Children().Count() < 1)
                {
                    return (false, $"Expected an array of length 1 or more as a parameter, got the following: {child.Children().Count()}");
                }

                if (child.Children().Any(p => p.Type == JTokenType.Array))
                {
                    return (false, "Did not expect any nested arrays as parameters.");
                }
            }

            return (true, string.Empty);
        }
    }
}
