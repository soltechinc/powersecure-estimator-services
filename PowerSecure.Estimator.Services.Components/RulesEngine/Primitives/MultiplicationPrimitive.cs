using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class MultiplicationPrimitive : IPrimitive
    {
        public string Name => "*";

        public int ParameterCount => 2;

        public decimal Invoke(params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
