using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
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
        public ISet<string> CallStack { get; set; } = new HashSet<string>();

        private UnresolvedParameter(JToken jToken, UnresolvedParameter parentParameter)
        {
            Token = jToken;
            Parameters = parentParameter.Parameters;
            Functions = parentParameter.Functions;
            ReferenceDataRepository = parentParameter.ReferenceDataRepository;
            InstructionSetRepository = parentParameter.InstructionSetRepository;
            EffectiveDate = parentParameter.EffectiveDate;
            Log = parentParameter.Log;
            CallStack = parentParameter.CallStack;
        }

        public object ToInstructionSet(object parameter, string searchString)
        {
            string value;
            switch(parameter)
            {
                case string s:
                    {
                        value = $"\"{s.ToStringLiteral()}\"";
                        break;
                    }
                case bool b:
                    {
                        value = parameter.ToString().ToLower();
                        break;
                    }
                default:
                    {
                        value = parameter.ToString();
                        break;
                    }
            }

            searchString = $"\"{searchString}\"";

            string json = Token.ToString(Newtonsoft.Json.Formatting.None).Replace(searchString,value);
            
            return new UnresolvedParameter(JToken.Parse(json), this).Resolve();
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
                                if(!Functions.ContainsKey(jProp.Name))
                                {
                                    Log?.LogError($"Primitive {jProp.Name} does not exist in primitive dictionary.");
                                    return null;
                                }
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

                                if(function.Name.StartsWith("is") || function.Name == "guard")
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
                                            
                                            if (keyParts.Length == 3 && keyParts[0].EndsWith("[]") && keyParts[1].EndsWith("[]"))
                                            { //cross-module submodule evaluation logic

                                                var moduleDataList = new List<object>();

                                                string moduleKey = $"{keyParts[0].Replace("[]", string.Empty)}";
                                                if (!Parameters.ContainsKey(moduleKey))
                                                {
                                                    Log?.LogWarning($"Unable to find module {moduleKey}");

                                                    return moduleDataList.ToArray();
                                                }

                                                var modules = (List<Dictionary<string, object>>)Parameters[moduleKey];
                                                
                                                string submoduleKey = $"{moduleKey}.{keyParts[1].Replace("[]", string.Empty)}";
                                                string instructionSetKey = $"{submoduleKey}.{keyParts[2]}";
                                                var keysToEvaluate = new List<string>() { instructionSetKey };

                                                foreach (var module in modules.ToList())
                                                {
                                                    foreach (var k in Parameters.Keys)
                                                    {
                                                        if (!k.Contains(".") && !module.ContainsKey(k) && k != moduleKey)
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
                                                        else if (k == "all.effectivedate" && !module.ContainsKey(k))
                                                        {
                                                            module.Add(k, Parameters[k]);
                                                        }
                                                        else if (k == "all.usstate" && !module.ContainsKey(k))
                                                        {
                                                            module.Add(k, Parameters[k]);
                                                        }
                                                    }

                                                    var moduleParameters = new Dictionary<string, object>(module.Where(p => p.Value != null));
                                                    var submoduleDataList = new List<object>();
                                                    if (!moduleParameters.ContainsKey(submoduleKey))
                                                    {
                                                        Log?.LogWarning($"Unable to find submodule {submoduleKey}");
                                                        continue;
                                                    }

                                                    var submodules = (List<Dictionary<string, object>>)moduleParameters[submoduleKey];

                                                    foreach (var submodule in submodules)
                                                    {
                                                        if (submodule.ContainsKey("currentsubmodule") && ((bool)submodule["currentsubmodule"]))
                                                        {
                                                            continue;
                                                        }

                                                        if (!submodule.ContainsKey(instructionSetKey) || submodule[instructionSetKey] == null)
                                                        {
                                                            var submoduleDataSheet = new Dictionary<string, object>(moduleParameters);
                                                            submoduleDataSheet.Remove(submoduleKey);

                                                            foreach (var pair in submodule)
                                                            {
                                                                if(!submoduleDataSheet.ContainsKey(pair.Key))
                                                                {
                                                                    submoduleDataSheet.Add(pair.Key, pair.Value);
                                                                }
                                                                else
                                                                {
                                                                    submoduleDataSheet[pair.Key] = pair.Value;
                                                                }
                                                            }
                                                            if (!submodule.ContainsKey(instructionSetKey))
                                                            {
                                                                submoduleDataSheet.Add(instructionSetKey, null);
                                                                submodule.Add(instructionSetKey, null);
                                                            }
                                                            
                                                            var returnedDataSheet = new RulesEngine().EvaluateDataSheet(submoduleDataSheet, keysToEvaluate, EffectiveDate, Functions, InstructionSetRepository, ReferenceDataRepository, Log, CallStack);

                                                            foreach (var returnedKey in submodule.Keys.ToList())
                                                            {
                                                                if (returnedKey.Contains("."))
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
                                                                else
                                                                {
                                                                    submodule.Remove(returnedKey);
                                                                }
                                                            }
                                                        }

                                                        if (submodule.ContainsKey(instructionSetKey) && submodule[instructionSetKey] != null)
                                                        {
                                                            submoduleDataList.Add(submodule[instructionSetKey]);
                                                        }
                                                    }
                                                    
                                                    moduleDataList.Add(submoduleDataList.ToArray());

                                                    foreach (var k in module.Keys.ToList())
                                                    {
                                                        if (!k.Contains("."))
                                                        {
                                                            module.Remove(k);
                                                        }
                                                    }
                                                }

                                                Parameters.Add(key, moduleDataList.ToArray());
                                            }
                                            else if (keyParts.Length > 1 && keyParts[0].EndsWith("[]"))
                                            {
                                                //module array logic
                                                var moduleDataList = new List<object>();

                                                string moduleKey = $"{keyParts[0].Replace("[]", string.Empty)}";
                                                if (!Parameters.ContainsKey(moduleKey))
                                                {
                                                    Log?.LogWarning($"Unable to find module {moduleKey}");

                                                    return moduleDataList.ToArray();
                                                }

                                                var modules = (List<Dictionary<string, object>>)Parameters[moduleKey];
                                                string moduleDataKey = $"{moduleKey}.{keyParts[1]}";
                                                if(keyParts.Length == 3)
                                                {
                                                    moduleDataKey = moduleDataKey + $".{keyParts[2]}";
                                                }
                                                var keysToEvaluate = new List<string>() { moduleDataKey };

                                                foreach (var module in modules.ToList())
                                                {
                                                    if (!module.ContainsKey(moduleDataKey) || module[moduleDataKey] == null)
                                                    {
                                                        foreach (var k in Parameters.Keys)
                                                        {
                                                            if (!k.Contains(".") && !module.ContainsKey(k) && k != moduleKey)
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
                                                            else if (k == "all.effectivedate" && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                            else if (k == "all.usstate" && !module.ContainsKey(k))
                                                            {
                                                                module.Add(k, Parameters[k]);
                                                            }
                                                        }

                                                        var moduleParameters = new Dictionary<string, object>(module.Where(p => p.Value != null));

                                                        if (!moduleParameters.ContainsKey(moduleDataKey))
                                                        {
                                                            moduleParameters.Add(moduleDataKey, null);
                                                        }
                                                        if (!module.ContainsKey(moduleDataKey))
                                                        {
                                                            module.Add(moduleDataKey, null);
                                                        }

                                                        var returnedDataSheet = new RulesEngine().EvaluateDataSheet(moduleParameters, keysToEvaluate, EffectiveDate, Functions, InstructionSetRepository, ReferenceDataRepository, Log, CallStack);
                                                        foreach (var returnedKey in module.Keys.ToList())
                                                        {
                                                            if (returnedKey.Contains("."))
                                                            {
                                                                if (returnedDataSheet.ContainsKey(returnedKey) && returnedDataSheet[returnedKey] != null)
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
                                                            else
                                                            {
                                                                module.Remove(returnedKey);
                                                            }
                                                        }

                                                        foreach (var k in module.Keys.ToList())
                                                        {
                                                            if (!k.Contains("."))
                                                            {
                                                                module.Remove(k);
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
                                                var submoduleDataList = new List<object>();

                                                string submoduleKey = $"{keyParts[0]}.{keyParts[1].Replace("[]", string.Empty)}";
                                                if (!Parameters.ContainsKey(submoduleKey))
                                                {
                                                    Log?.LogWarning($"Unable to find submodule {submoduleKey}");

                                                    return submoduleDataList.ToArray();
                                                }

                                                var submodules = (List<Dictionary<string, object>>)Parameters[submoduleKey];
                                                string submoduleDataKey = $"{submoduleKey}.{keyParts[2]}";
                                                var keysToEvaluate = new List<string>() { submoduleDataKey };

                                                foreach (var submodule in submodules)
                                                {
                                                    if(submodule.ContainsKey("currentsubmodule") && ((bool)submodule["currentsubmodule"]))
                                                    {
                                                        continue;
                                                    }

                                                    if(!submodule.ContainsKey(submoduleDataKey) || submodule[submoduleDataKey] == null)
                                                    {
                                                        var submoduleDataSheet = new Dictionary<string, object>(Parameters);
                                                        submoduleDataSheet.Remove(submoduleKey);

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
                                                        if(!submodule.ContainsKey(submoduleDataKey))
                                                        {
                                                            submoduleDataSheet.Add(submoduleDataKey, null);
                                                            submodule.Add(submoduleDataKey, null);
                                                        }
                                                        var returnedDataSheet = new RulesEngine().EvaluateDataSheet(submoduleDataSheet, keysToEvaluate, EffectiveDate, Functions, InstructionSetRepository, ReferenceDataRepository, Log, CallStack);
                                                        foreach (var returnedKey in submodule.Keys.ToList())
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
                                                        foreach(var returnedKey in returnedDataSheet.Keys)
                                                        {
                                                            if (returnedDataSheet[returnedKey] != null && !submodule.Keys.Contains(returnedKey))
                                                            {
                                                                if (!Parameters.ContainsKey(returnedKey))
                                                                {
                                                                    Parameters.Add(returnedKey, returnedDataSheet[returnedKey]);
                                                                }
                                                                else if (Parameters[returnedKey] == null)
                                                                {
                                                                    Parameters[returnedKey] = returnedDataSheet[returnedKey];
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
                                        Parameters[key] = childInstructionSet?.Evaluate(Parameters, Functions, ReferenceDataRepository, InstructionSetRepository, EffectiveDate, Log, CallStack);
                                    }

                                    if (Parameters[key] is object[] o && o.Length == 2 
                                        && o[0] is object[] o2 && o2.Length == 2 
                                        && o2[0] != null && o2[0].ToRawString() == "--No Selection--"
                                        && string.IsNullOrEmpty(o2[1]?.ToString()) 
                                        && o[1] is object[] o3 && o3.Length == 2)
                                    {
                                        return o3[1];
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
