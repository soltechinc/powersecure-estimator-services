using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public interface IPrimitive
    {
        string Name { get; }
        
        object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository);

        (bool Success, string Message) Validate(JToken jToken);
    }
}
