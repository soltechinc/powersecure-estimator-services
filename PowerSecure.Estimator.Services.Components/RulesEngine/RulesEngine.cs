using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine : IRulesEngine
    {
        public IDictionary<string, object> EvaluateDataSheet(string dataSheet, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository)
        {
            var suppliedParameters = new HashSet<string>();
            var missingParameters = new HashSet<string>();

            var stack = new Stack<Dictionary<string, object>>();
            stack.Push(new Dictionary<string, object>());
            List<JToken> order = new List<JToken>();
            JObject.Parse(dataSheet).WalkNodes(
                PreOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JArray jArray:
                            {
                                var parameters = stack.Peek();
                                var arrayParameters = new Dictionary<string, object>();
                                parameters.Add(jArray.Path, arrayParameters);
                                stack.Push(arrayParameters);
                                break;
                            }
                        case JObject jObject:
                            {
                                var parameters = stack.Peek();
                                var objParameters = new Dictionary<string, object>();
                                parameters.Add(jObject.Path, objParameters);
                                stack.Push(objParameters);
                                break;
                            }
                    }
                },
                Visit: jToken =>
                {
                    var parameters = stack.Peek();

                    string path = CleanPath(jToken.Path);

                    switch (jToken.Type)
                    {
                        case JTokenType.Boolean:
                            parameters.Add(path, jToken.ToObject<bool>());
                            break;
                        case JTokenType.Integer:
                            parameters.Add(path, Convert.ToDecimal(jToken.ToObject<int>()));
                            break;
                        case JTokenType.Float:
                            parameters.Add(path, Convert.ToDecimal(jToken.ToObject<float>()));
                            break;
                        case JTokenType.Null:
                            parameters.Add(path, null);
                            break;
                        default:
                            parameters.Add(path, jToken.ToString());
                            break;
                    }
                },
                PostOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JArray jArray:
                            {
                                stack.Pop();
                                break;
                            }
                        case JObject jObject:
                            {
                                if(stack.Count > 1)
                                {
                                    stack.Pop();
                                }
                                break;
                            }
                    }
                });
            {
                var parameters = stack.Pop();
                /*
                foreach (var parameter in dataSheet)
                {
                    if(parameter.Value == null)
                    {
                        missingParameters.Add(parameter.Key.Trim().ToLower());
                    }
                    else
                    {
                        suppliedParameters.Add(parameter.Key.Trim().ToLower());
                    }
                    parameters.Add(parameter.Key?.Trim()?.ToLower(), parameter.Value);
                }*/

                var childInstructionSetKeys = new HashSet<string>();
                var instructionSets = new Dictionary<string, IInstructionSet>();
                var neededParameters = new HashSet<string>();
                foreach (var instructionSet in instructionSetRepository.SelectByKey(missingParameters, effectiveDate))
                {
                    instructionSets.Add(instructionSet.Name, instructionSet);
                    foreach (var childInstructionSetKey in instructionSet.ChildInstructionSets)
                    {
                        if (!childInstructionSetKeys.Contains(childInstructionSetKey) && !suppliedParameters.Contains(childInstructionSetKey))
                        {
                            childInstructionSetKeys.Add(childInstructionSetKey);
                        }
                    }
                    foreach (var parameter in instructionSet.Parameters)
                    {
                        if (!neededParameters.Contains(parameter))
                        {
                            neededParameters.Add(parameter);
                        }
                    }
                }

                foreach (var instructionSet in instructionSetRepository.SelectByKey(childInstructionSetKeys, effectiveDate))
                {
                    if (!instructionSets.ContainsKey(instructionSet.Key))
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

                if (!neededParameters.All(p => suppliedParameters.Contains(p)))
                {
                    throw new InvalidOperationException("Missing required parameters");
                }

                foreach (var instructionSet in instructionSets.Values)
                {
                    if (!parameters.ContainsKey(instructionSet.Key))
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
                        parameters[key] = instructionSet.Evaluate(parameters, functions, referenceDataRepository);
                    }
                    //dataSheet[key] = parameters[key].ToRawString();
                }
            }

            return null;
        }

        private static string CleanPath(string path)
        {
            return path.Replace("['", "").Replace("']", "");
        }
    }
}
