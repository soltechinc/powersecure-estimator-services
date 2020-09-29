using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class FormatPrimitive : IFunction
    {
        public string Name => "format";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            if (parameters.Length == 1)
            {
                var d = parameters[0].ToDecimal();
                if (d == null)
                {
                    return 0.ToStringLiteral();
                }
                else
                {
                    return d.Value.ToString("N2").ToStringLiteral();
                }
            }
            return parameters.ToDecimal().Select(d => d.HasValue ? d.Value.ToString("N2") : "0").ToStringLiteral().Select(o => (object)o).ToArray();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
