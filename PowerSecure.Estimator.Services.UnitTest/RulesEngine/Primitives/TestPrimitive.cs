using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    public class TestPrimitive : IPrimitive
    {
        private ParamsFunc _func;

        public TestPrimitive(string name, int parameterCount, ParamsFunc func)
        {
            Name = name;
            ParameterCount = parameterCount;
            _func = func;
        }

        public string Name { get; private set; }

        public int ParameterCount { get; private set; }

        public decimal Invoke(params object[] parameters)
        {
            return _func.Invoke(parameters);
        }
    }
}
