using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public interface IRulesEngine
    {
        IDictionary<string, string> EvaluateDataSheet(IDictionary<string, string> dataSheet, IDictionary<string, IPrimitive> primitives, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository dataSheetRepository);
    }
}
