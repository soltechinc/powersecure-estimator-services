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
        public InstructionSet(string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets)
        {
            Name = name;
            Instructions = instructions;
            Parameters = new ReadOnlyCollection<string>(parameters.ToList());
            ChildInstructionSets = new ReadOnlyCollection<string>(childInstructionSets.ToList());
        }

        public string Name { get; private set; }

        public string Instructions { get; private set; }

        public ReadOnlyCollection<string> Parameters { get; private set; }

        public ReadOnlyCollection<string> ChildInstructionSets { get; private set; }

        public static void InsertNew(string instructionSetName, string instructionDefinition, IInstructionSetRepository repository, IDictionary<string, IPrimitive> primitives
            )
        {
            if (instructionSetName == null) throw new ArgumentNullException("instructionSetName");
            if (instructionDefinition == null) throw new ArgumentNullException("instructionDefinition");
            if (repository == null) throw new ArgumentNullException("repository");
            if (primitives == null) throw new ArgumentNullException("primitives");

            var terminals = new SortedSet<string>();

            JObject.Parse(instructionDefinition).WalkNodes(jObject =>
            {
                var nameList = jObject.Properties().Select(p => p.Name).ToList();

                if (nameList.Count != 1)
                {
                    throw new InvalidOperationException($"Expected one primitive, found {nameList.Count}");
                }

                var name = nameList.First();
                IPrimitive primitive;
                if(!primitives.TryGetValue(name.ToLower(), out primitive))
                {
                    throw new InvalidOperationException($"The following token is not a defined primitive: {name}");
                }
                
                var value = jObject.GetValue(name);

                if (value.Type != JTokenType.Array)
                {
                    throw new InvalidOperationException($"Expected a parameter array, got the following: {value.ToString()}");
                }
                
                if(value.Children().Count() != primitive.ParameterCount)
                {
                    throw new InvalidOperationException($"Expected a parameter array of length {primitive.ParameterCount}, got the following: {value.Children().Count()}");
                }

                value.Children().Where(p => p.Type != JTokenType.Object && p.Type != JTokenType.Array).ForEach(child => terminals.Add(child.ToString()));
            });

            //divide terminals into classes
            var parameters = new List<string>();
            var childInstructionSets = new List<string>();

            foreach (string terminal in terminals)
            {
                if (Decimal.TryParse(terminal, out Decimal d))
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

            repository.Insert(new InstructionSet(instructionSetName, instructionDefinition, parameters, childInstructionSets));

            //update existing instruction sets 
            repository.SelectByParameter(instructionSetName)
                      .ForEach(instructionSet => repository.Update(new InstructionSet(instructionSet.Name,
                        instructionSet.Instructions,
                        instructionSet.Parameters.Where(x => x != instructionSetName),
                        instructionSet.ChildInstructionSets.Union(new List<string> { instructionSetName }))));
        }
    }
}
