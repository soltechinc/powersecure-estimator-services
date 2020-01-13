using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class MarginPrimitive : IPrimitive
    {
        public string Name => "margin";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = Primitive.ConvertToDecimal(parameters);

            var price = decimals[0];
            var cost = decimals[1];
            var applyMargin = decimals.Length == 3 ? decimals[2] : 1;

            if(price <= 0)
            {
                return 0;
            }

            return ((price - cost) / price) * applyMargin;
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2 && jToken.Children().Count() != 3)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 2 or 3, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
