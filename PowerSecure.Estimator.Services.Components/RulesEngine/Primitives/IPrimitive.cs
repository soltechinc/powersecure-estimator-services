using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public interface IFunction
    {
        string Name { get; }

        object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository);

        (bool Success, string Message) Validate(JToken jToken);
    }
}
