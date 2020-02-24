using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using static PowerSecure.Estimator.Services.Components.RulesEngine.Primitives.Primitive;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    public class TestPrimitive : IFunction
    {
        public delegate (bool Success, string Message) ValidationFunc(JToken jToken);

        private readonly Func<object[], IReferenceDataRepository, decimal> _invokeFunc;
        private readonly ValidationFunc _validateFunc;

        public TestPrimitive(string name, Func<object[], IReferenceDataRepository, decimal> invokeFunc, ValidationFunc validateFunc)
        {
            Name = name;
            _invokeFunc = invokeFunc;
            _validateFunc = validateFunc;
        }

        public string Name { get; private set; }
        
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
