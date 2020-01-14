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
            JToken[] children = jToken.Children().ToArray();

            if (children.Count() != 3)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 3, got the following: {jToken.Children().Count()}");
            }

            if(children[0].Type == JTokenType.Array || children[2].Type == JTokenType.Array)
            {
                return Tuple.Create(false, "Did not expect an array as the first or third parameter.");
            }

            if (children[1].Type != JTokenType.Array)
            {
                return Tuple.Create(false, "Expected criteria array as the second parameter.");
            }

            if (children[1].Children().Count() == 0)
            {
                return Tuple.Create(false, "Expected entries in the criteria array.");
            }

            if (children[1].Children().Count() % 2 != 0)
            {
                return Tuple.Create(false, "Expected an even number of entries in the criteria array.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
