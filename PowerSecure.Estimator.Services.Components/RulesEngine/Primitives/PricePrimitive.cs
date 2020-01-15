// 2, 3, or 4 parameters. Parameter 1 is cost, parameter 2 is tax, parameter 3 is margin, parameter 4 is a boolean specifying whether to apply the margin
using System;
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
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = parameters.ToDecimal().ToArray();

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

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2 && jToken.Children().Count() != 3 && jToken.Children().Count() != 4)
            {
                return (false, $"Expected a parameter array of length 2, 3, or 4, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
