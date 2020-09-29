using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class PercentPrimitive : IFunction
    {
        public string Name => "percent";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters.Length == 1 ?
                parameters[0].ToDecimal()?.ToString("P2")?.ToStringLiteral() :
                parameters[0].ToDecimal()?.ToString($"P{parameters[1].ToDecimal() ?? 0}")?.ToStringLiteral();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1 || jToken.Children().Count() > 2)
            {
                return (false, $"Expected a parameter array of length 1 or 2, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
