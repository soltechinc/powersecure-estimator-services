using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public interface IInstructionSetRepository
    {
        IInstructionSet Get(string module, string name, DateTime effectiveDate);

        IInstructionSet Get(string key, DateTime effectiveDate);
    }
}
