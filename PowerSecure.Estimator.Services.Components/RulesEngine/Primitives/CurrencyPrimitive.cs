using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class CurrencyPrimitive : IFunction
    {
        public string Name => "curr";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var obj = parameters[0].ToResolvedParameter();
            switch(obj)
            {
                case object[] arr:
                    {
                        return arr.Select(o => (object)$"${o.ToDecimal():C}").ToArray();
                    }
                default:
                    return $"${obj.ToDecimal():C}";
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 1)
            {
                return (false, $"Expected a parameter array of length 1, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
