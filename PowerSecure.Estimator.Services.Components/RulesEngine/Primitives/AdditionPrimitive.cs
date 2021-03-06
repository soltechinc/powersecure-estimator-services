﻿// 1 or more parameters
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Collections.Generic;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class AdditionPrimitive : IFunction
    {
        public string Name => "+";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            object[] resolvedParameters = parameters.Select(o => o.ToResolvedParameter()).ToArray();

            if (resolvedParameters.Length > 1)
            {
                if (resolvedParameters.All(o => o is object[]))
                {
                    var list = new List<object>();
                    foreach (object o in resolvedParameters)
                    {
                        list.AddRange((object[])o);
                    }
                    return list.ToArray();
                }
                else
                {
                    return resolvedParameters.ToDecimal().Sum();
                }
            }

            switch (resolvedParameters[0])
            {
                case object[] objs:
                    {
                        return objs.ToDecimal().Sum();
                    }
                default:
                    {
                        return resolvedParameters[0].ToDecimal();
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
