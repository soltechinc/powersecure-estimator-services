using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    public class InMemoryRepository : IInstructionSetRepository
    {
        public Dictionary<string, InstructionSet> Items { get; } = new Dictionary<string, InstructionSet>();

        public void Insert(InstructionSet instructionSet)
        {
            if (Items.ContainsKey(instructionSet.Name))
            {
                throw new Exception();
            }

            Items.Add(instructionSet.Name, instructionSet);
        }
    
        public void Update(InstructionSet instructionSet)
        {
            if(!Items.ContainsKey(instructionSet.Name))
            {
                throw new Exception();
            }

            Items[instructionSet.Name] = instructionSet;
        }

        public bool ContainsKey(string key)
        {
            return Items.ContainsKey(key);
        }

        public IEnumerable<InstructionSet> SelectByKey(params string[] instructionSetNames)
        {
            return SelectByKey(instructionSetNames.AsEnumerable());
        }

        public IEnumerable<InstructionSet> SelectByKey(IEnumerable<string> instructionSetNames)
        {
            foreach (string instructionSetName in instructionSetNames)
            {
                if (Items.TryGetValue(instructionSetName, out InstructionSet instructionSet))
                {
                    yield return instructionSet;
                }
                else
                {
                    //handle error condition
                }
            }
        }

        public IEnumerable<InstructionSet> SelectByParameter(string parameter)
        {
            return Items.Select(x => x.Value)
                        .Where(x => x.Parameters.Contains(parameter))
                        .ToList(); /* have to project to a new list to allow dictionary modification*/
        }
    }
}
