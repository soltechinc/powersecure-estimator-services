using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class ItemPrimitive : IFunction
    {
        public string Name => "item";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            object[] array = parameters[0].ToObjectArray();
            int index = (int)parameters[1].ToDecimal().Value;
            return array[index];
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
