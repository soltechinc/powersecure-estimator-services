﻿// 1 or more parameters.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SubtractionPrimitive : IPrimitive
    {
        public string Name => "-";
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            if (parameters.Length > 1)
            {
                return parameters.ToDecimal().Aggregate((decimal?)null,(acc, current) => acc == null ? current : acc - current);
            }

            switch (parameters[0])
            {
                case object[] objs:
                    {
                        return objs.ToDecimal().Aggregate((decimal?)null, (acc, current) => acc == null ? current : acc - current);
                    }
                default:
                    {
                        return -parameters[0].ToDecimal();
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
