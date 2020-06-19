using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine
    {
        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log)
        {
            var missingParameters = new HashSet<string>();

            foreach (var parameter in dataSheet)
            {
                if (parameter.Value is List<Dictionary<string, object>> submodules)
                {
                    bool addedMissingParam = false;
                    foreach (var submodule in submodules)
                    {
                        foreach (var submoduleParameter in submodule)
                        {
                            if (submoduleParameter.Value == null)
                            {
                                missingParameters.Add(parameter.Key.Trim().ToLower());
                                addedMissingParam = true;
                            }

                            if (addedMissingParam)
                            {
                                break;
                            }
                        }
                        if (addedMissingParam)
                        {
                            break;
                        }
                    }
                }
                else if (parameter.Value == null)
                {
                    missingParameters.Add(parameter.Key.Trim().ToLower());
                }
            }
            
            return EvaluateDataSheet(dataSheet, missingParameters, effectiveDate, functions, instructionSetRepository, referenceDataRepository, log, new HashSet<string>());
        }

        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, IEnumerable<string> keysToEvaluate, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log, ISet<string> callStack)
        {
            var missingParameters = new HashSet<string>();
            var parameters = new Dictionary<string, object>();
            
            foreach (var parameter in dataSheet)
            {
                if(keysToEvaluate.Contains(parameter.Key) && (parameter.Value == null || parameter.Value is List<Dictionary<string, object>>))
                {
                    missingParameters.Add(parameter.Key.Trim().ToLower());
                }
                else
                {
                    parameters.Add(parameter.Key?.Trim()?.ToLower(), parameter.Value);
                }
            }

            foreach (var key in missingParameters)
            {
                if(!parameters.ContainsKey(key))
                {
                    if (dataSheet[key] is List<Dictionary<string, object>> submodules)
                    { //submodule evaluation
                        var baseDataSheet = new Dictionary<string, object>(dataSheet);
                        baseDataSheet.Remove(key);
                        foreach(var submodule in submodules)
                        {
                            var submoduleDataSheet = new Dictionary<string, object>(baseDataSheet);
                            foreach(var pair in submodule)
                            {
                                submoduleDataSheet.Add(pair.Key, pair.Value);
                            }
                            var returnedDataSheet = EvaluateDataSheet(submoduleDataSheet, submodule.Keys, effectiveDate, functions, instructionSetRepository, referenceDataRepository, log, callStack);
                            foreach(var submoduleKey in submodule.Keys.ToList())
                            {
                                submodule[submoduleKey] = returnedDataSheet[submoduleKey];
                            }
                        }

                        parameters.Add(key, dataSheet[key]);
                    }
                    else
                    {
                        IInstructionSet instructionSet = instructionSetRepository.Get(key, effectiveDate);
                        if (instructionSet == null)
                        {
                            log?.LogWarning($"Unable to find instruction set {key}");
                        }

                        parameters.Add(key, instructionSet?.Evaluate(parameters, functions, referenceDataRepository, instructionSetRepository, effectiveDate, log, callStack));
                    }
                }

                dataSheet[key] = parameters[key];
            }

            return dataSheet;
        }
    }
}
