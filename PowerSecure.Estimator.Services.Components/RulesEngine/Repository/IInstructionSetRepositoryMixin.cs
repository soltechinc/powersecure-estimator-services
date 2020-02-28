using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public static class IInstructionSetRepositoryMixin
    {
        public static IInstructionSet ValidateInstructionSet(this IInstructionSetRepository repository, string instructionSetModule, string instructionSetName, string instructionDefinition, DateTime startDate, DateTime creationDate, Func<string, string,string,string, DateTime, DateTime, IInstructionSet> instructionSetFactory, IDictionary<string, IFunction> functions)
        {
            if (instructionSetModule == null) throw new ArgumentNullException("instructionSetModule");
            if (instructionSetName == null) throw new ArgumentNullException("instructionSetName");
            if (instructionDefinition == null) throw new ArgumentNullException("instructionDefinition");
            if (repository == null) throw new ArgumentNullException("repository");
            if (instructionSetFactory == null) throw new ArgumentNullException("instructionSetFactory");
            if (functions == null) throw new ArgumentNullException("functions");
            
            instructionSetName = instructionSetName.Trim().ToLower();
            instructionSetModule = instructionSetModule.Trim().ToLower();

            JObject.Parse(instructionDefinition).WalkNodes(
            PreOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                var nameList = jObject.Properties().Select(p => p.Name).ToList();

                                if (nameList.Count != 1)
                                {
                                    throw new InvalidOperationException($"Expected one primitive, found {nameList.Count}");
                                }

                                var name = nameList[0];

                                var value = jObject.GetValue(name);

                                if (value.Type != JTokenType.Array)
                                {
                                    throw new InvalidOperationException($"Expected a parameter array, got the following: {value.ToString()}");
                                }

                                if (!functions.TryGetValue(name.Trim().ToLower(), out IFunction function))
                                {
                                    break; //if we don't know the name of the primitive, maybe it'll be loaded later. In any case, we cannot validate it
                                }

                                (bool isValid, string message) = function.Validate(value);

                                if (!isValid)
                                {
                                    throw new InvalidOperationException(message);
                                }
                                break;
                            }
                    }
                }
            );

            return instructionSetFactory(Guid.NewGuid().ToString(), instructionSetModule, instructionSetName, instructionDefinition, startDate, creationDate);
        }
    }
}
