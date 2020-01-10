using System;
using System.Collections.Generic;
using System.Text;

namespace powersecure_instruction_set_engine.Repository
{
    public interface IInstructionSetRepository
    {
        IEnumerable<InstructionSet> SelectByKey(params string[] instructionSetNames);

        IEnumerable<InstructionSet> SelectByParameter(string parameter);

        void Insert(InstructionSet instructionSet);

        void Update(InstructionSet instructionSet);

        bool ContainsKey(string key);
    }
}
