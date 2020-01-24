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
        public InstructionSet(string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets)
        {
            Module = module;
            Name = name;
            Instructions = instructions;
            Parameters = new ReadOnlyCollection<string>(parameters.ToList());
            ChildInstructionSets = new ReadOnlyCollection<string>(childInstructionSets.ToList());
        }

        public string Module { get; private set; }

        public string Name { get; private set; }

        public string Key => $"{Module}.{Name}";

        public string Instructions { get; private set; }

        public ReadOnlyCollection<string> Parameters { get; private set; }

        public ReadOnlyCollection<string> ChildInstructionSets { get; private set; }

        public static InstructionSet Create(string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets)
        {
            return new InstructionSet(module, name, instructions, parameters, childInstructionSets);
        }
    }
}
