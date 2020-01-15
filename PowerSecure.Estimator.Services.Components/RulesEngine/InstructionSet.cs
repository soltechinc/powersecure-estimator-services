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
    public class InstructionSet : IInstructionSet
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

        public static InstructionSet Create(string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, int sequence)
        {
            return new InstructionSet(name, instructions, parameters, childInstructionSets, sequence);
        }
    }
}
