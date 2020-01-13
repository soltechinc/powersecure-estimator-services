using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public class InstructionSet
    {
        public InstructionSet(string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, int sequence)
        {
            Name = name;
            Instructions = instructions;
            Parameters = new ReadOnlyCollection<string>(parameters.ToList());
            ChildInstructionSets = new ReadOnlyCollection<string>(childInstructionSets.ToList());
            Sequence = sequence;
        }

        public string Name { get; private set; }

        public string Instructions { get; private set; }

        public ReadOnlyCollection<string> Parameters { get; private set; }

        public ReadOnlyCollection<string> ChildInstructionSets { get; private set; }

        public int Sequence { get; private set; }

        public decimal Evaluate(IDictionary<string, string> parameters, IDictionary<string, IPrimitive> primitives, IReferenceDataRepository referenceDataRepository)
        {
            EvaluationNode rootNode = null;
            EvaluationNode currentNode = null;

            JObject.Parse(Instructions).DoubleWalkNodes(jToken =>
            {
                if(!(jToken.Type == JTokenType.Object || (jToken.Type == JTokenType.Array && jToken.Parent.Type == JTokenType.Array)))
                {
                    return;
                }

                var node = new EvaluationNode();
                node.Parent = currentNode;
                if (currentNode != null)
                {
                    ((Tuple<IPrimitive, List<EvaluationNode>>)currentNode.Value).Item2.Add(node);
                }
                if (rootNode == null)
                {
                    rootNode = node;
                }
                currentNode = node;

                switch (jToken)
                {
                    case JObject jObject:
                        {
                            var primitive = primitives[jObject.Properties().Select(p => p.Name).First()];
                            node.Value = Tuple.Create(primitive, new List<EvaluationNode>());
                            break;
                        }
                    case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                        {
                            node.Value = Tuple.Create((IPrimitive)null, new List<EvaluationNode>());
                            break;
                        }
                }
            },
            jToken =>
            {
                if (!(jToken.Type == JTokenType.Object || (jToken.Type == JTokenType.Array && jToken.Parent.Type == JTokenType.Array)))
                {
                    return;
                }

                var node = currentNode;
                
                switch (jToken)
                {
                    case JObject jObject:
                        {
                            var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)node.Value;
                            var value = tuple.Item1.Invoke(tuple.Item2.Select(p => p.Value).ToArray(), referenceDataRepository);
                            node.Value = value.ToString();
                            break;
                        }
                    case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                        {
                            var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)node.Value;
                            node.Value = tuple.Item2.Select(p => p.Value.ToString()).ToArray();
                            break;
                        }
                }

                currentNode = node.Parent;
            },
            jToken =>
            {
                var node = new EvaluationNode();
                var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)currentNode.Value;
                tuple.Item2.Add(node);
                
                if (jToken.Type == JTokenType.String && !(tuple.Item1 == null || !tuple.Item1.ResolveParameters))
                {
                    node.Value = parameters[jToken.ToString()];
                }
                else
                {
                    node.Value = jToken.ToString();
                }
            });

            return decimal.Parse(rootNode.Value.ToString());
        }

        public static void InsertNew(string instructionSetName, string instructionDefinition, IInstructionSetRepository repository, IDictionary<string, IPrimitive> primitives)
        {
            if (instructionSetName == null) throw new ArgumentNullException("instructionSetName");
            if (instructionDefinition == null) throw new ArgumentNullException("instructionDefinition");
            if (repository == null) throw new ArgumentNullException("repository");
            if (primitives == null) throw new ArgumentNullException("primitives");

            var terminals = new SortedSet<string>();

            JObject.Parse(instructionDefinition).WalkNodes(jToken =>
            {
                switch(jToken)
                {
                    case JObject jObject:
                        {
                            var nameList = jObject.Properties().Select(p => p.Name).ToList();

                            if (nameList.Count != 1)
                            {
                                throw new InvalidOperationException($"Expected one primitive, found {nameList.Count}");
                            }

                            var name = nameList.First();

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
            jToken =>
            {
                terminals.Add(jToken.ToString());
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
            foreach(InstructionSet instructionSet in repository.SelectByKey(childInstructionSets))
            {
                if(instructionSet.Sequence > maxSequence)
                {
                    maxSequence = instructionSet.Sequence;
                }
            }
            
            repository.Insert(new InstructionSet(instructionSetName, instructionDefinition, parameters, childInstructionSets, maxSequence + 1));

            //update existing instruction sets 
            repository.SelectByParameter(instructionSetName)
                      .ForEach(instructionSet => repository.Update(new InstructionSet(instructionSet.Name,
                        instructionSet.Instructions,
                        instructionSet.Parameters.Where(x => x != instructionSetName),
                        instructionSet.ChildInstructionSets.Union(new List<string> { instructionSetName }),
                        Math.Max(instructionSet.Sequence, maxSequence + 2))));
        }
    }
}
