using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public interface IInstructionSetRepository
    {
        IEnumerable<InstructionSet> SelectByKey(params string[] instructionSetNames);

        IEnumerable<InstructionSet> SelectByKey(IEnumerable<string> instructionSetNames);

        IEnumerable<InstructionSet> SelectByParameter(string parameter);

        void Insert(InstructionSet instructionSet);

        void Update(InstructionSet instructionSet);

        bool ContainsKey(string key);
    }
}
