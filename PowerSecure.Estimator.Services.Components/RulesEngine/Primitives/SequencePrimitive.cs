using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    class SequencePrimitive : IFunction
    {
        public string Name => "seq";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return Enumerable.Range((int)parameters[0].ToDecimal(), (int)parameters[1].ToDecimal()).Select(x => (object)x).ToArray();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            return (true, string.Empty);
        }
    }
}
