﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public class UnresolvedParameter
    {
        public UnresolvedParameter() { }

        public JToken Token { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public IDictionary<string, IFunction> Functions { get; set; }
        public IReferenceDataRepository ReferenceDataRepository { get; set; }
        public IInstructionSetRepository InstructionSetRepository { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsNullValue { get; private set; } = false;
        public ILogger Log { get; set; }

        private UnresolvedParameter(JToken jToken, UnresolvedParameter parentParameter)
        {
            Token = jToken;
            Parameters = parentParameter.Parameters;
            Functions = parentParameter.Functions;
            ReferenceDataRepository = parentParameter.ReferenceDataRepository;
            InstructionSetRepository = parentParameter.InstructionSetRepository;
            EffectiveDate = parentParameter.EffectiveDate;
            Log = parentParameter.Log;
        }

        public object Resolve()
        {
            if(IsNullValue)
            {
                return null;
            }

            object value = new Func<object>(() =>
                {
                    //convert object to primitive or scalar
                    switch (Token.Type)
                    {
                        case JTokenType.Object:
                            { //this is a primitive
                                var jProp = Token.Children<JProperty>().First();
                                var unresolvedParameters = jProp.Value.Children().Select(jToken => new UnresolvedParameter(jToken, this)).ToArray();
                                object retValue = null;
                                IFunction function = Functions[jProp.Name];
                                try
                                {
                                    retValue = function.Invoke(unresolvedParameters, ReferenceDataRepository);
                                }
                                catch (Exception ex)
                                {
                                    Log?.LogWarning(ex, $"Exception while attempting to run primitive {jProp.Name}");
                                    return null;
                                }

                                if(function.Name.StartsWith("is"))
                                {
                                    return retValue;
                                }
                                return unresolvedParameters.Any(p => p.IsNullValue) ? null : retValue;
                            }
                        case JTokenType.Array when Token.Parent.Type == JTokenType.Array:
                            {
                                //this is an array, resolve all parameters
                                return Token.Children().Select(jToken => new UnresolvedParameter(jToken, this)).ToArray();
                            }
                        case JTokenType.Array:
                            {
                                break;
                            }
                        case JTokenType.String:
                            {
                                string str = Token.ToString();
                                if (string.IsNullOrEmpty(str) || str.StartsWith('$')) //these are string literals
                                {
                                    return str;
                                }
                                else //these should be resolved against the parameters
                                {
                                    string key = str.Trim().ToLower();

                                    if (!Parameters.ContainsKey(key))
                                    {
                                        if (key.Contains("[]"))
                                        {
                                            string[] keyParts = key.Split(".");
                                            if (keyParts.Length > 1 && keyParts[0].EndsWith("[]"))
                                            {
                                                //module array logic
                                                string moduleKey = $"{keyParts[0].Replace("[]", string.Empty)}";
                                                if (!Parameters.ContainsKey(moduleKey))
                                                {
                                                    Log?.LogWarning($"Unable to find module {moduleKey}");

                                                    return null;
                                                }

                                                var modules = (List<Dictionary<string, object>>)Parameters[moduleKey];
                                                string moduleDataKey = $"{moduleKey}.{keyParts[1]}";
                                                if(keyParts.Length == 3)
                                                {
                                                    moduleDataKey = moduleDataKey + $".{keyParts[2]}";
                                                }
                                                var keysToEvaluate = new List<string>() { moduleDataKey };
                                                var moduleDataList = new List<object>();

                                                foreach (var module in modules.ToList())
                                                {
                                                    if (!module.ContainsKey(moduleDataKey) || module[moduleDataKey] == null)
                                                    {
                                                        foreach (var k in Parameters.Keys)
                                                        {
                                                            if (!k.Contains(".") && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                            else if (k == "all.projecttype" && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                            else if (k == "all.outsideequipmentpercentage" && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                            else if (k == "all.desiredinstallrate" && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                        }

                                                        if(!module.ContainsKey(moduleDataKey))
                                                        {
                                                            module.Add(moduleDataKey, null);
                                                        }
                                                        var returnedDataSheet = new RulesEngine().EvaluateDataSheet(module, keysToEvaluate, EffectiveDate, Functions, InstructionSetRepository, ReferenceDataRepository, Log);
                                                        foreach (var returnedKey in module.Keys)
                                                        {
                                                            if (returnedDataSheet[returnedKey] != null)
                                                            {
                                                                if (!module.ContainsKey(returnedKey))
                                                                {
                                                                    module.Add(returnedKey, returnedDataSheet[returnedKey]);
                                                                }
                                                                else if (module[returnedKey] == null)
                                                                {
                                                                    module[returnedKey] = returnedDataSheet[returnedKey];
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (module.ContainsKey(moduleDataKey) && module[moduleDataKey] != null)
                                                    {
                                                        moduleDataList.Add(module[moduleDataKey]);
                                                    }
                                                }

                                                Parameters.Add(key, moduleDataList.ToArray());
                                            }
                                            else if (keyParts.Length == 3 && keyParts[1].EndsWith("[]"))
                                            {
                                                //submodule array logic
                                                string submoduleKey = $"{keyParts[0]}.{keyParts[1].Replace("[]", string.Empty)}";
                                                if (!Parameters.ContainsKey(submoduleKey))
                                                {
                                                    Log?.LogWarning($"Unable to find submodule {submoduleKey}");

                                                    return null;
                                                }

                                                var submodules = (List<Dictionary<string, object>>)Parameters[submoduleKey];
                                                var baseDataSheet = new Dictionary<string, object>(Parameters);
                                                baseDataSheet.Remove(submoduleKey);
                                                string submoduleDataKey = $"{submoduleKey}.{keyParts[2]}";
                                                var keysToEvaluate = new List<string>() { submoduleDataKey };
                                                var submoduleDataList = new List<object>();

                                                foreach (var submodule in submodules)
                                                {
                                                    if(!submodule.ContainsKey(submoduleDataKey) || submodule[submoduleDataKey] == null)
                                                    {
                                                        var submoduleDataSheet = new Dictionary<string, object>(baseDataSheet);
                                                        foreach (var pair in submodule)
                                                        {
                                                            submoduleDataSheet.Add(pair.Key, pair.Value);
                                                        }
                                                        if(!submodule.ContainsKey(submoduleDataKey))
                                                        {
                                                            submoduleDataSheet.Add(submoduleDataKey, null);
                                                        }
                                                        var returnedDataSheet = new RulesEngine().EvaluateDataSheet(submoduleDataSheet, keysToEvaluate, EffectiveDate, Functions, InstructionSetRepository, ReferenceDataRepository, Log);
                                                        foreach (var returnedKey in submodule.Keys)
                                                        {
                                                            if (returnedDataSheet[returnedKey] != null)
                                                            {
                                                                if (!submodule.ContainsKey(returnedKey))
                                                                {
                                                                    submodule.Add(returnedKey, returnedDataSheet[returnedKey]);
                                                                }
                                                                else if (submodule[returnedKey] == null)
                                                                {
                                                                    submodule[returnedKey] = returnedDataSheet[returnedKey];
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if(submodule.ContainsKey(submoduleDataKey) && submodule[submoduleDataKey] != null)
                                                    {
                                                        submoduleDataList.Add(submodule[submoduleDataKey]);
                                                    }
                                                }

                                                Parameters.Add(key, submoduleDataList.ToArray());
                                            }
                                            else
                                            {
                                                Log?.LogWarning($"Invalid array parsing for {key}");

                                                return null;
                                            }
                                        }
                                        else
                                        {
                                            var instructionSet = InstructionSetRepository.Get(key, EffectiveDate);

                                            if (instructionSet == null)
                                            {
                                                Log?.LogWarning($"Attempted to find instruction set {key} but failed");

                                                return null;
                                            }

                                            Parameters.Add(key, instructionSet);
                                        }
                                    }
                                    if (Parameters[key] is IInstructionSet childInstructionSet)
                                    {
                                        Log?.LogInformation($"Running instruction set {key}");

                                        Parameters[key] = childInstructionSet?.Evaluate(Parameters, Functions, ReferenceDataRepository, InstructionSetRepository, EffectiveDate, Log);
                                    }
                                    if(Parameters[key] is object[] o && o.Length == 1 && o[0] is object[] o2 && o2.Length == 2)
                                    {
                                        return o2[1];
                                    }

                                    return Parameters[key];
                                }
                            }
                        case JTokenType.Null:
                            {
                                return null;
                            }
                        case JTokenType.Boolean:
                            {
                                return bool.Parse(Token.ToString());
                            }
                        default:
                            {
                                return decimal.Parse(Token.ToString());
                            }
                    }
                    return null;
                })();

            if(value == null)
            {
                IsNullValue = true;
            }
            return value;
        }
    }
}
