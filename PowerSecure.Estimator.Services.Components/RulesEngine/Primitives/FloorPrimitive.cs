﻿// 2 parameters A and B has to be greater than 0 and A has to be greater than B. Quotient equals Divide A to B then add 1. The final results is the value of Quotient multiplied by B
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives {
    public class FloorPrimitive : IFunction {
        public string Name => "floor";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository) {
            var value = parameters[0].ToDecimal();
            var multiple = parameters[1].ToDecimal();
            if(multiple == 0)
            {
                return 0;
            }
            
            return (int)Math.Floor(value / multiple) * multiple;
        }

        public (bool Success, string Message) Validate(JToken jToken) {
            if(jToken.Children().Count() != 2) {
                return (false, $"Expected a parameter array of length 2, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array)) {
                if (jToken.Children().Count() > 1) {
                    return (false, "Did not expect any arrays as parameters.");
                }

                var child = jToken.Children().First();
                if (child.Children().Count() < 2) {
                    return (false, $"Expected an array of length 2 or more as a parameter, got the following: {child.Children().Count()}");
                }

                if (child.Children().Any(p => p.Type == JTokenType.Array)) {
                    return (false, "Did not expect any nested arrays as parameters.");
                }
            }                       
            return (true, string.Empty);
        }
    }
}
