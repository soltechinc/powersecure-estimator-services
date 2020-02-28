using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    public class TestInstructionSet : IInstructionSet
    {
        public TestInstructionSet(string id, string module, string name, string instructions, DateTime startDate, DateTime creationDate)
        {
            Id = id;
            Module = module;
            Name = name;
            Instructions = instructions;
            StartDate = startDate;
            CreationDate = creationDate;
        }

        public string Id { get; private set; }

        public string Module { get; private set; }

        public string Name { get; private set; }

        public string Key => $"{Module}.{Name}";

        public string Instructions { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime CreationDate { get; private set; }

        public static TestInstructionSet Create(string id, string module, string name, string instructions, DateTime startDate, DateTime creationDate)
        {
            return new TestInstructionSet(id, module, name, instructions, startDate, creationDate);
        }
    }
}
