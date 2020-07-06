using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using Microsoft.Extensions.Logging;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class IInstructionSetMixin
    {
        public static object Evaluate(this IInstructionSet instructionSet, IDictionary<string, object> parameters, IDictionary<string, IFunction> functions, IReferenceDataRepository referenceDataRepository, IInstructionSetRepository instructionSetRepository, DateTime effectiveDate, ILogger log, ISet<string> callStack)
        {
            if(callStack.Contains(instructionSet.Name))
            {
                return null;
            }

            JToken token = null;
            try
            {
                token = JObject.Parse(instructionSet.Instructions);
            }
            catch(Exception ex)
            {
                log.LogError($"Error while parsing instruction set {instructionSet.Name}", ex);
                return null;
            }

            callStack.Add(instructionSet.Name);
            var obj = new UnresolvedParameter() { Token = token, Parameters = parameters, Functions = functions, ReferenceDataRepository = referenceDataRepository, InstructionSetRepository = instructionSetRepository, EffectiveDate = effectiveDate, Log = log, CallStack = callStack }.Resolve();
            callStack.Remove(instructionSet.Name);
            return obj;
        }
    }
}
