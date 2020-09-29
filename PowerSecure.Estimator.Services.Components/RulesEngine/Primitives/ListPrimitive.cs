// 1 or more parameters
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class ListPrimitive : IFunction
    {
        public string Name => "list";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return parameters.ToObjectArray().Select(o => o.ToResolvedParameter()).ToArray();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return (false, "Did not expect any nested arrays as parameters.");
            }

            return (true, string.Empty);
        }
    }
}
