using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public class EvaluationNode
    {
        public EvaluationNode Parent { get; set; }
        public object Value { get; set; }
    }
}
