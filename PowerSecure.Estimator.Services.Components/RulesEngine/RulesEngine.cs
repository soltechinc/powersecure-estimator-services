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
            JObject.Parse(dataSheet).WalkNodes(
                PreOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                var parameters = stack.Peek();
                                jObject.Properties()
                                    .Where(p => p.Value.Type != JTokenType.Object && p.Value.Type != JTokenType.Array)
                                    .Aggregate(parameters, (acc, prop) =>
                                     {
                                         switch (prop.Value.Type)
                                         {
                                             case JTokenType.Boolean:
                                                 acc.Add(prop.Path, prop.Value.ToObject<bool>());
                                                 break;
                                             case JTokenType.Integer:
                                                 acc.Add(prop.Path, Convert.ToDecimal(prop.Value.ToObject<int>()));
                                                 break;
                                             case JTokenType.Float:
                                                 acc.Add(prop.Path, Convert.ToDecimal(prop.Value.ToObject<float>()));
                                                 break;
                                             case JTokenType.Null:
                                                 acc.Add(prop.Path, null);
                                                 break;
                                             default:
                                                 acc.Add(prop.Path, prop.Value.ToString());
                                                 break;
                                         }
                                         return acc;
                                     });
                                break;
                            }
                        case JArray jArray:
                            {
                                
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
    }
}
