﻿// 2 parameters. Both parameters are equal-sized arrays of numerics.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SumProductPrimitive : IPrimitive
    {
        public string Name => "sumproduct";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var values = Primitive.ConvertToDecimal((string[])parameters[0]);
            var factors = Primitive.ConvertToDecimal((string[])parameters[1]);

            decimal value = 0;

            for (int i = 0; i < values.Length; ++i)
            {
                value += values[i] * factors[i];
            }

            return value;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2)
            {
                return (false, $"Expected a parameter array of length 2, got the following: {jToken.Children().Count()}");
            }

            if (!jToken.Children().All(p => p.Type == JTokenType.Array))
            {
                return (false, "Expected all parameters to be arrays.");
            }

            var list = new List<JToken>(jToken.Children());
            if(list[0].Children().Count() != list[1].Children().Count())
            {
                return (false, "Expected parameter arrays to be equal in length.");
            }

            if(list[0].Children().Count() == 0)
            {
                return (false, "Expected parameter arrays to have entries.");
            }

            return (true, string.Empty);
        }
    }
}
