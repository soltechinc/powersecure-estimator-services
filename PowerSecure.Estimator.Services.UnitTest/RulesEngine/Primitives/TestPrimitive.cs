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
        public delegate (bool Success, string Message) ValidationFunc(JToken jToken);

        private Func<object[], IReferenceDataRepository, decimal> _invokeFunc;
        private ValidationFunc _validateFunc;

        public TestPrimitive(string name, bool resolveParameters, Func<object[], IReferenceDataRepository, decimal> invokeFunc, ValidationFunc validateFunc)
        {
            Name = name;
            ResolveParameters = resolveParameters;
            _invokeFunc = invokeFunc;
            _validateFunc = validateFunc;
        }

        public string Name { get; private set; }

        public bool ResolveParameters { get; private set; }

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return _invokeFunc?.Invoke(parameters, referenceDataRepository) ?? decimal.MinValue;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            return _validateFunc?.Invoke(jToken) ?? (false, "No validation logic");
        }
    }
}
