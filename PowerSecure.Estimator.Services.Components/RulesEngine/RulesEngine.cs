using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine : IRulesEngine
    {
        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, DateTime effectiveDate, IDictionary<string, IPrimitive> primitives, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository)
        {
            var suppliedParameters = new HashSet<string>();
            var missingParameters = new HashSet<string>();

            foreach(var parameter in dataSheet)
            {
                if(parameter.Value == null)
                {
                    missingParameters.Add(parameter.Key.Trim().ToLower());
                }
                else
                {
                    suppliedParameters.Add(parameter.Key.Trim().ToLower());
                }
            }

            var childInstructionSetKeys = new HashSet<string>();
            var instructionSets = new Dictionary<string, IInstructionSet>();
            var neededParameters = new HashSet<string>();
            foreach(var instructionSet in instructionSetRepository.SelectByKey(missingParameters, effectiveDate))
            {
                instructionSets.Add(instructionSet.Name, instructionSet);
                foreach(var childInstructionSetKey in instructionSet.ChildInstructionSets)
                {
                    if(!childInstructionSetKeys.Contains(childInstructionSetKey) && !suppliedParameters.Contains(childInstructionSetKey))
                    {
                        childInstructionSetKeys.Add(childInstructionSetKey);
                    }
                }
                foreach(var parameter in instructionSet.Parameters)
                {
                    if(!neededParameters.Contains(parameter))
                    {
                        neededParameters.Add(parameter);
                    }
                }
            }

            foreach(var instructionSet in instructionSetRepository.SelectByKey(childInstructionSetKeys, effectiveDate))
            {
                if(!instructionSets.ContainsKey(instructionSet.Key))
                {
                    instructionSets.Add(instructionSet.Key, instructionSet);
                }
                foreach (var parameter in instructionSet.Parameters)
                {
                    if (!neededParameters.Contains(parameter))
                    {
                        neededParameters.Add(parameter);
                    }
                }
            }

            if(!neededParameters.All(p => suppliedParameters.Contains(p)))
            {
                throw new InvalidOperationException("Missing required parameters");
            }

            var parameters = new Dictionary<string, object>();
            foreach(var pair in dataSheet)
            {
                parameters.Add(pair.Key?.Trim()?.ToLower(), pair.Value);
            }
            
            foreach (var instructionSet in instructionSets.Values)
            {
                if(!parameters.ContainsKey(instructionSet.Key))
                {
                    parameters.Add(instructionSet.Key, instructionSet);
                }
                else if (parameters[instructionSet.Key] == null)
                {
                    parameters[instructionSet.Key] = instructionSet;
                }
            }

            foreach (var key in missingParameters)
            {
                if (parameters[key] is IInstructionSet instructionSet)
                {
                    parameters[key] = instructionSet.Evaluate(parameters, primitives, referenceDataRepository);
                }
                dataSheet[key] = parameters[key].ToRawString();
            }

            return dataSheet;
        }
    }
}
