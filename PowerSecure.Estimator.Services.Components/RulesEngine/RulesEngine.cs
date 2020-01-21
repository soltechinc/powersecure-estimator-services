using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine : IRulesEngine
    {
        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, IDictionary<string, IPrimitive> primitives, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository)
        {
            var suppliedParameters = new HashSet<string>() { "true", "false" };
            var missingParameters = new HashSet<string>();

            foreach(var parameter in dataSheet)
            {
                if(parameter.Value == null)
                {
                    missingParameters.Add(parameter.Key);
                }
                else
                {
                    suppliedParameters.Add(parameter.Key);
                }
            }

            var childInstructionSetNames = new HashSet<string>();
            var instructionSets = new Dictionary<string, IInstructionSet>();
            var neededParameters = new HashSet<string>();
            foreach(var instructionSet in instructionSetRepository.SelectByKey(missingParameters))
            {
                instructionSets.Add(instructionSet.Name, instructionSet);
                foreach(var childInstructionSetName in instructionSet.ChildInstructionSets)
                {
                    if(!childInstructionSetNames.Contains(childInstructionSetName) && !suppliedParameters.Contains(childInstructionSetName))
                    {
                        childInstructionSetNames.Add(childInstructionSetName);
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

            foreach(var instructionSet in instructionSetRepository.SelectByKey(childInstructionSetNames))
            {
                if(!instructionSets.ContainsKey(instructionSet.Name))
                {
                    instructionSets.Add(instructionSet.Name, instructionSet);
                }
                foreach (var parameter in instructionSet.Parameters)
                {
                    if (!neededParameters.Contains(parameter))
                    {
                        neededParameters.Add(parameter);
                    }
                }
            }

            if(!neededParameters.All(p => suppliedParameters.Select(s => s.ToLower()).Contains(p.ToLower())))
            {
                throw new InvalidOperationException("Missing required parameters");
            }

            var parameters = new Dictionary<string, object>();
            foreach(var pair in dataSheet)
            {
                parameters.Add(pair.Key?.ToLower(), pair.Value);
            }
            parameters["true"] = "1";
            parameters["false"] = "0";
            
            foreach (var instructionSet in instructionSets.Values)
            {
                if(!parameters.ContainsKey(instructionSet.Name))
                {
                    parameters.Add(instructionSet.Name, instructionSet);
                }
                else if (parameters[instructionSet.Name] == null)
                {
                    parameters[instructionSet.Name] = instructionSet;
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
