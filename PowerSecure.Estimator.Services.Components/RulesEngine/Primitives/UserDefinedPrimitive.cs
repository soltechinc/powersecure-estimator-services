using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class UserDefinedPrimitive : IFunction
    {
        public string Name => throw new NotImplementedException();

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            throw new NotImplementedException();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            throw new NotImplementedException();
        }
    }
}
