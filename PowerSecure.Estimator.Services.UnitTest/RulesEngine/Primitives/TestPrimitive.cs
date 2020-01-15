using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    public class TestPrimitive : IPrimitive
    {
        public delegate (bool success, string message) ValidationFunc(JToken jToken);

        private ParamsFunc _paramsFunc;
        private ValidationFunc _validationFunc;

        public TestPrimitive(string name, bool resolveParameters, ParamsFunc paramsFunc, ValidationFunc validationFunc)
        {
            Name = name;
            ResolveParameters = resolveParameters;
            _paramsFunc = paramsFunc;
            _validationFunc = validationFunc;
        }

        public string Name { get; private set; }

        public bool ResolveParameters { get; private set; }

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return _paramsFunc.Invoke(parameters);
        }

        public (bool success, string message) Validate(JToken jToken)
        {
            return _validationFunc.Invoke(jToken);
        }
    }
}
