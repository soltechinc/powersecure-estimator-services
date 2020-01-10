using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Interfaces {
    public interface IRulesEngine
    {
        IDictionary<string, string> EvaluateDataSheet(IDictionary<string, string> dataSheet);
    }
}
