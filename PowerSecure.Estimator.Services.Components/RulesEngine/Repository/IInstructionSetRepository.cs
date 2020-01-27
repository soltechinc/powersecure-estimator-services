using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public interface IInstructionSetRepository
    {
        IEnumerable<IInstructionSet> SelectByKey(IEnumerable<string> instructionSetNames, DateTime date);

        IEnumerable<IInstructionSet> SelectByParameter(string parameter);

        void Insert(IInstructionSet instructionSet);

        void Update(IInstructionSet instructionSet);

        bool ContainsKey(string key);
    }
}
