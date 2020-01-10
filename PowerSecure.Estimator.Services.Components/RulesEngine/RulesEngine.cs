using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine : IRulesEngine
    {
        public IDictionary<string, string> EvaluateDataSheet(IDictionary<string, string> dataSheet, IInstructionSetRepository instructionSetRepository, IDataSheetRepository dataSheetRepository)
        {

            return null;
        }
    }
}
