using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class FindPrimitive : IPrimitive
    {
        public string Name => "find";

        public bool ResolveParameters => false;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return referenceDataRepository.Lookup((string)parameters[0], (string[])parameters[1], (string)parameters[2]);
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 3)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 3, got the following: {jToken.Children().Count()}");
            }

            JToken array = jToken.Children().ElementAt(1);
            if (array.Type != JTokenType.Array)
            {
                return Tuple.Create(false, "Expected criteria array as the second parameter.");
            }

            if(array.Children().Count() % 2 != 0)
            {
                return Tuple.Create(false, "Expected an even number of entries in the criteria array.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
