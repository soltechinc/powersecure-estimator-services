﻿// 2 or more parameters
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

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            decimal value = decimal.MinValue;
            parameters.ToDecimal().ForEach(p => {
                if (value == decimal.MinValue)
                {
                    value = p;
                }
                else
                {
                    value /= p;
                }
            });
            return value;
        }

        public (bool success, string message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 2)
            {
                return (false, $"Expected a parameter array of length 2 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
