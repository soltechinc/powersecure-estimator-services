using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine : IRulesEngine
    {
        public IDictionary<string, string> EvaluateDataSheet(IDictionary<string, string> dataSheet, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository dataSheetRepository)
        {
            var suppliedParameters = new HashSet<string>();
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
            var instructionSets = new Dictionary<string, InstructionSet>();
            var neededParameters = new HashSet<string>();
            foreach(var instructionSet in instructionSetRepository.SelectByKey(dataSheet.Where(p => p.Value == null).Select(p => p.Key)))
            {
                instructionSets.Add(instructionSet.Name, instructionSet);
                foreach(var childInstructionSetName in instructionSet.ChildInstructionSets)
                {
                    if(!childInstructionSetNames.Contains(childInstructionSetName))
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

            if(!neededParameters.All(p => suppliedParameters.Contains(p)))
            {
                throw new InvalidOperationException("Missing required parameters");
            }

            while(instructionSets.Count != 0)
            {
                foreach(var key in instructionSets.Keys.ToList())
                {
                    var instructionSet = instructionSets[key];

                    bool hasParams = true;
                    foreach(var parameter in instructionSet.Parameters)
                    {
                        if (!suppliedParameters.Contains(parameter))
                        {
                            hasParams = false;
                        }
                    }

                    if(hasParams)
                    {

                    }
                }
            }

            return null;
        }
    }
}
