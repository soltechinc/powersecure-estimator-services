// 2 or 3 parameters. Parameter 1 is price, parameter 2 is cost, parameter 3 is an optionally-applied margin.
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class MarginPrimitive : IFunction
    {
        public string Name => "margin";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = parameters.ToDecimal().ToArray();

            var price = decimals[0];
            var cost = decimals[1];
            var applyMargin = decimals.Length == 3 ? decimals[2] : 1;

            if (price <= 0)
            {
                return 0;
            }

            return ((price - cost) / price) * applyMargin;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2 && jToken.Children().Count() != 3)
            {
                return (false, $"Expected a parameter array of length 2 or 3, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
