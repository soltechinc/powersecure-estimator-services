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
        public static void InsertNew(this IInstructionSetRepository repository, string instructionSetName, string instructionDefinition, Func<string,string, IEnumerable<string>, IEnumerable<string>, int, IInstructionSet> instructionSetFactory, IDictionary<string, IPrimitive> primitives)
        {
            if (instructionSetName == null) throw new ArgumentNullException("instructionSetName");
            if (instructionDefinition == null) throw new ArgumentNullException("instructionDefinition");
            if (repository == null) throw new ArgumentNullException("repository");
            if (instructionSetFactory == null) throw new ArgumentNullException("instructionSetFactory");
            if (primitives == null) throw new ArgumentNullException("primitives");

            var terminals = new HashSet<string>();

            JObject.Parse(instructionDefinition).WalkNodes(jToken =>
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

                            if (!primitives.TryGetValue(name.ToLower(), out IPrimitive primitive))
                            {
                                throw new InvalidOperationException($"The following token is not a defined primitive: {name}");
                            }

                            var value = jObject.GetValue(name);

                            if (value.Type != JTokenType.Array)
                            {
                                throw new InvalidOperationException($"Expected a parameter array, got the following: {value.ToString()}");
                            }

                            (bool isValid, string message) = primitive.Validate(value);

                            if (!isValid)
                            {
                                throw new InvalidOperationException(message);
                            }
                            break;
                        }
                }
            },
            null,
            jToken =>
            {
                terminals.Add(jToken.ToString().ToLower());
            });

            //divide terminals into classes
            var parameters = new List<string>();
            var childInstructionSets = new List<string>();

            foreach (var terminal in terminals)
            {
                if (decimal.TryParse(terminal, out decimal d))
                {
                    continue;
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

            int maxSequence = -1;
            foreach (IInstructionSet instructionSet in repository.SelectByKey(childInstructionSets))
            {
                if (instructionSet.Sequence > maxSequence)
                {
                    maxSequence = instructionSet.Sequence;
                }
            }

            repository.Insert(instructionSetFactory(instructionSetName.ToLower(), instructionDefinition, parameters, childInstructionSets, maxSequence + 1));

            //update existing instruction sets 
            repository.SelectByParameter(instructionSetName)
                      .ForEach(instructionSet => repository.Update(instructionSetFactory(instructionSet.Name,
                        instructionSet.Instructions,
                        instructionSet.Parameters.Where(x => x != instructionSetName),
                        instructionSet.ChildInstructionSets.Union(new List<string> { instructionSetName }),
                        Math.Max(instructionSet.Sequence, maxSequence + 2))));
        }
    }
}
