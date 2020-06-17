using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    class MapPrimitive : IFunction
    {
        public string Name => "map";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            object obj = parameters[0].ToResolvedParameter();

            switch (obj)
            {
                case object[] objs:
                    {
                        return objs.Select(o => parameters[1].ToInstructionSet(o)).ToArray();
                    }
                default:
                    {
                        return parameters[1].ToInstructionSet(obj);
                    }
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            return (true, string.Empty);
        }
    }
}
