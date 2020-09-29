using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    class MapPrimitive : IFunction
    {
        public string Name => "map";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            object obj = parameters[0].ToResolvedParameter();
            string searchString = parameters[1].ToStringLiteral();

            switch (obj)
            {
                case object[] objs:
                    {
                        return objs.Select(o => parameters[2].ToInstructionSet(o, searchString)).ToArray();
                    }
                default:
                    {
                        return parameters[2].ToInstructionSet(obj, searchString);
                    }
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            return (true, string.Empty);
        }
    }
}
