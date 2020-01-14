﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class PricePrimitive : IPrimitive
    {
        public string Name => "price";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = Primitive.ConvertToDecimal(parameters);

            var cost = decimals[0];
            var tax = decimals[1];
            var margin = decimals.Length >= 3 ? decimals[2] : 0;
            var applyMargin = decimals.Length == 4 ? decimals[3] : 1;

            if (cost <= 0)
            {
                return 0;
            }

            return (cost/(1-(margin * applyMargin)))+tax;
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2 && jToken.Children().Count() != 3 && jToken.Children().Count() != 4)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 2, 3, or 4, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}