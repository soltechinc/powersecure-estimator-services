﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public class RulesEngine
    {
        public class UnresolvedKey
        {
            public readonly static UnresolvedKey Instance = new UnresolvedKey();
        }

        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log)
        {
            dataSheet.Apply(x => x.Item2 ?? UnresolvedKey.Instance);
            log.LogInformation("Data sheet to calculate: " + JToken.FromObject(dataSheet));

            var missingParameters = new HashSet<string>();

            foreach (var parameter in dataSheet)
            {
                if (parameter.Value is List<Dictionary<string, object>> submodules && parameter.Key.Contains("."))
                {
                    bool addedMissingParam = false;
                    foreach (var submodule in submodules)
                    {
                        foreach (var submoduleParameter in submodule)
                        {
                            if (submoduleParameter.Value == UnresolvedKey.Instance)
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
                else if (parameter.Value == UnresolvedKey.Instance)
                {
                    missingParameters.Add(parameter.Key.Trim().ToLower());
                }
            }

            log.LogInformation("Keys to evaluate: " + JToken.FromObject(missingParameters));
            dataSheet = EvaluateDataSheet(dataSheet, missingParameters, effectiveDate, functions, instructionSetRepository, referenceDataRepository, log, new HashSet<string>());

            log.LogInformation("Returned data sheet: " + JToken.FromObject(dataSheet));
            dataSheet.Apply(x => x.Item2 == RulesEngine.UnresolvedKey.Instance ? null : x.Item2);
            return dataSheet;
        }

        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, IEnumerable<string> keysToEvaluate, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log, ISet<string> callStack)
        {
            var missingParameters = new HashSet<string>();
            var parameters = new Dictionary<string, object>();

            log.LogInformation("Temp keys to evaluate: " + JToken.FromObject(keysToEvaluate));

            foreach (var parameter in dataSheet)
            {
                if (keysToEvaluate.Contains(parameter.Key))
                {
                    if (parameter.Value == UnresolvedKey.Instance)
                    {
                        missingParameters.Add(parameter.Key.Trim().ToLower());
                    }
                    else if (parameter.Value is List<Dictionary<string, object>> listValue)
                    {
                        missingParameters.Add(parameter.Key.Trim().ToLower());
                        if (parameter.Key.Contains("."))
                        {   //submodules
                            var newListValue = new List<Dictionary<string, object>>();
                            foreach (var dict in listValue)
                            {
                                var newDict = new Dictionary<string, object>();
                                foreach (var pair in dict)
                                {
                                    if (pair.Value != null)
                                    {
                                        newDict.Add(pair.Key.Trim().ToLower(), pair.Value);
                                    }
                                }
                                newListValue.Add(newDict);
                            }
                            parameters.Add(parameter.Key.Trim().ToLower(), newListValue);
                        }
                        else
                        {   //modules
                            parameters.Add(parameter.Key.Trim().ToLower(), parameter.Value);
                        }
                    }
                    else
                    {
                        parameters.Add(parameter.Key.Trim().ToLower(), parameter.Value);
                    }
                }
                else
                {
                    parameters.Add(parameter.Key.Trim().ToLower(), parameter.Value);
                }
            }

            foreach (var key in missingParameters)
            {
                if (dataSheet[key] is List<Dictionary<string, object>> submodules)
                { //submodule evaluation
                    var baseDataSheet = new Dictionary<string, object>(dataSheet);
                    baseDataSheet.Remove(key);
                    string submodulePrefix = $"{key}.";
                    foreach (var submodule in submodules)
                    {
                        var submoduleDataSheet = new Dictionary<string, object>(baseDataSheet);
                        foreach (var pair in submodule)
                        {
                            if (!submoduleDataSheet.ContainsKey(pair.Key))
                            {
                                submoduleDataSheet.Add(pair.Key, pair.Value);
                            }
                            else
                            {
                                submoduleDataSheet[pair.Key] = pair.Value;
                            }
                        }
                        var returnedDataSheet = EvaluateDataSheet(submoduleDataSheet, submodule.Keys, effectiveDate, functions, instructionSetRepository, referenceDataRepository, log, callStack);
                        foreach(var returnedKey in returnedDataSheet.Keys.ToList())
                        {
                            if (returnedDataSheet[returnedKey] == RulesEngine.UnresolvedKey.Instance)
                            {
                                continue;
                            }

                            if (returnedKey.StartsWith(submodulePrefix))
                            {
                                if (!submodule.ContainsKey(returnedKey))
                                {
                                    submodule.Add(returnedKey, returnedDataSheet[returnedKey]);
                                    log?.LogDebug($"Datasheet {returnedKey} evaluated to value {submodule[returnedKey] ?? "{null}"}");
                                }
                                else if (submodule[returnedKey] == RulesEngine.UnresolvedKey.Instance)
                                {
                                    submodule[returnedKey] = returnedDataSheet[returnedKey];
                                    log?.LogDebug($"Datasheet {returnedKey} evaluated to value {submodule[returnedKey] ?? "{null}"}");
                                }
                            }
                            else
                            {
                                if (!parameters.ContainsKey(returnedKey))
                                {
                                    parameters.Add(returnedKey, returnedDataSheet[returnedKey]);
                                    log?.LogDebug($"Datasheet {returnedKey} evaluated to value {parameters[returnedKey] ?? "{null}"}");
                                }
                                else if (parameters[returnedKey] == RulesEngine.UnresolvedKey.Instance)
                                {
                                    parameters[returnedKey] = returnedDataSheet[returnedKey];
                                    log?.LogDebug($"Datasheet {returnedKey} evaluated to value {parameters[returnedKey] ?? "{null}"}");
                                }
                            }
                        }
                    }

                    if (parameters.ContainsKey(key))
                    {
                        parameters[key] = dataSheet[key];
                        log?.LogDebug($"Datasheet {key} evaluated to value {parameters[key] ?? "{null}"}");
                    }
                    else
                    {
                        parameters.Add(key, dataSheet[key]);
                        log?.LogDebug($"Datasheet {key} evaluated to value {parameters[key] ?? "{null}"}");
                    }
                }
                else if (!parameters.ContainsKey(key))
                {
                    IInstructionSet instructionSet = instructionSetRepository.Get(key, effectiveDate);
                    if (instructionSet == null)
                    {
                        log?.LogWarning($"Unable to find instruction set {key}");
                    }

                    parameters.Add(key, instructionSet?.Evaluate(parameters, functions, referenceDataRepository, instructionSetRepository, effectiveDate, log, callStack));
                    log?.LogDebug($"Datasheet {key} evaluated to value {parameters[key] ?? "{null}"}");
                }

                dataSheet[key] = parameters[key];

                foreach (var dataSheetKey in dataSheet.Keys)
                {
                    if (!dataSheetKey.Contains("."))
                    {
                        //stripping modules out of modules
                        switch (dataSheet[dataSheetKey])
                        {
                            case List<Dictionary<string, object>> modules:
                                {
                                    foreach (var module in modules)
                                    {
                                        var list = module.Keys.Where(x => !x.Contains(".")).ToList();
                                        foreach (var str in list)
                                        {
                                            module.Remove(str);
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;

                        }
                    }
                }
            }

            return dataSheet;
        }
    }
}
