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
            object[] resolvedParameters = parameters.Select(o => o.ToResolvedParameter()).ToArray();

            int decimalLength = 2;

            if (resolvedParameters.Length > 1)
            {
                var d = resolvedParameters[1].ToDecimal();
                decimalLength = d.HasValue ? (int)d.Value : 2;
            }

            switch(resolvedParameters[0])
            {
                case object[] objs:
                    {
                        return objs.ToDecimal().Select(d => d.HasValue ? d.Value.ToString($"N{decimalLength}") : "0").ToStringLiteral().Select(o => (object)o).ToArray();
                    }
                default:
                    {
                        var d = resolvedParameters[0].ToDecimal();
                        return d.HasValue ? d.Value.ToString($"N{decimalLength}").ToStringLiteral() : 0.ToStringLiteral();
                    }
            }
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
