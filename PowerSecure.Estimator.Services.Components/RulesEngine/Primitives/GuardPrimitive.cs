﻿using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class GuardPrimitive : IFunction
    {
        public string Name => "guard";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            decimal? retValue = 0;
            if(parameters.Length > 1)
            {
                retValue = parameters[1].ToDecimal();
            }

            return parameters[0].ToResolvedParameter() ?? retValue;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() > 2 || jToken.Children().Count() == 0)
            {
                return (false, $"Expected a parameter array of length 1 or 2, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}