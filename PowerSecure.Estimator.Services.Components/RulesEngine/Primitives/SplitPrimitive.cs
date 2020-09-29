using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SplitPrimitive : IFunction
    {
        public string Name => "split";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            string str = parameters[0].ToRawString();
            string delimiter = parameters[1].ToRawString();
            return str.Split(delimiter).Select(x => (object)x).ToArray();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2)
            {
                return (false, $"Expected a parameter array of length 1, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
