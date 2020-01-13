using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    public class TestPrimitive : IPrimitive
    {
        public delegate Tuple<bool, string> ValidationFunc(JToken jToken);

        private ParamsFunc _paramsFunc;
        private ValidationFunc _validationFunc;

        public TestPrimitive(string name, ParamsFunc paramsFunc, ValidationFunc validationFunc)
        {
            Name = name;
            _paramsFunc = paramsFunc;
            _validationFunc = validationFunc;
        }

        public string Name { get; private set; }

        public decimal Invoke(params object[] parameters)
        {
            return _paramsFunc.Invoke(parameters);
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            return _validationFunc.Invoke(jToken);
        }
    }
}
