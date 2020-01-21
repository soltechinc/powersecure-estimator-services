using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class IInstructionSetMixin
    {
        public static object Evaluate(this IInstructionSet instructionSet, IDictionary<string, object> parameters, IDictionary<string, IPrimitive> primitives, IReferenceDataRepository referenceDataRepository)
        {
            return new UnresolvedParameter() { Token = JObject.Parse(instructionSet.Instructions), Parameters = parameters, Primitives = primitives, ReferenceDataRepository = referenceDataRepository }.Resolve();
        }
    }
}
