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
        public static IInstructionSet InsertNew(this IInstructionSetRepository repository, string instructionSetModule, string instructionSetName, string instructionDefinition, DateTime startDate, DateTime creationDate, Func<string, string,string,string, IEnumerable<string>, IEnumerable<string>, DateTime, DateTime, IInstructionSet> instructionSetFactory, IDictionary<string, IFunction> functions)
        {
            if (instructionSetModule == null) throw new ArgumentNullException("instructionSetModule");
            if (instructionSetName == null) throw new ArgumentNullException("instructionSetName");
            if (instructionDefinition == null) throw new ArgumentNullException("instructionDefinition");
            if (repository == null) throw new ArgumentNullException("repository");
            if (instructionSetFactory == null) throw new ArgumentNullException("instructionSetFactory");
            if (functions == null) throw new ArgumentNullException("functions");

            var terminals = new HashSet<string>();
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
                },
            Visit: jToken =>
                {
                    terminals.Add(jToken.ToString().Trim().ToLower());
                }
            );

            //divide terminals into classes
            var parameters = new List<string>();
            var childInstructionSets = new List<string>();
            bool hasPositionalParameters = false;

            foreach (var terminal in terminals)
            {
                if (string.IsNullOrEmpty(terminal) ||
                    terminal.StartsWith("$") ||
                    decimal.TryParse(terminal, out decimal d) ||
                    bool.TryParse(terminal, out bool b))
                {
                    continue;
                }

                if(terminal.StartsWith("@"))
                {
                    hasPositionalParameters = true;
                }

                if (repository.ContainsKey(terminal))
                {
                    childInstructionSets.Add(terminal);
                }
                else
                {
                    parameters.Add(terminal);
                }
            }

            if(hasPositionalParameters)
            {
                functions.Add($"{instructionSetModule}.{instructionSetName}", null);
                return null;
            }

            IInstructionSet newInstructionSet = instructionSetFactory(Guid.NewGuid().ToString(), instructionSetModule, instructionSetName, instructionDefinition, parameters, childInstructionSets, startDate, creationDate);
            repository.Insert(newInstructionSet);

            //update existing instruction sets 
            repository.SelectByParameter(newInstructionSet.Key)
                      .ForEach(instructionSet => repository.Update(instructionSetFactory(instructionSet.Id,
                        instructionSet.Module,
                        instructionSet.Name,
                        instructionSet.Instructions,
                        instructionSet.Parameters.Where(x => x != newInstructionSet.Key),
                        instructionSet.ChildInstructionSets.Union(new List<string> { newInstructionSet.Key }),
                        instructionSet.StartDate,
                        instructionSet.CreationDate)));

            return newInstructionSet;
        }
    }
}
