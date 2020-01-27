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
        public InstructionSet(Guid id, string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, DateTime startDate, DateTime creationDate)
        {
            Id = id;
            Module = module;
            Name = name;
            Instructions = instructions;
            Parameters = new ReadOnlyCollection<string>(parameters.ToList());
            ChildInstructionSets = new ReadOnlyCollection<string>(childInstructionSets.ToList());
            StartDate = startDate;
            CreationDate = creationDate;
        }

        public Guid Id { get; private set; }

        public string Module { get; private set; }

        public string Name { get; private set; }

        public string Key => $"{Module}.{Name}";

        public string Instructions { get; private set; }

        public ReadOnlyCollection<string> Parameters { get; private set; }

        public ReadOnlyCollection<string> ChildInstructionSets { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime CreationDate { get; private set; }

        public static InstructionSet Create(Guid id, string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, DateTime startDate, DateTime creationDate)
        {
            return new InstructionSet(id, module, name, instructions, parameters, childInstructionSets, startDate, creationDate);
        }
    }
}
