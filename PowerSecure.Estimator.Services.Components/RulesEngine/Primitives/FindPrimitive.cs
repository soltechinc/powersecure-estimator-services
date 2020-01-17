// 3 parameters. Parameter 1 is the data set name, Parmeter 2 is the criteria array, Parameter 3 is the return field name
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
        
        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return referenceDataRepository.Lookup(parameters[0].ToRawString(), ((object[])((UnresolvedParameter)parameters[1]).Resolve()).Select(p => (object[])p).Select(p => (p[0].ToRawString(), p[1].ToRawString())).ToArray(), parameters[2].ToRawString());
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            JToken[] children = jToken.Children().ToArray();

            if (children.Count() != 3)
            {
                return (false, $"Expected a parameter array of length 3, got the following: {jToken.Children().Count()}");
            }

            if(children[0].Type == JTokenType.Array || children[2].Type == JTokenType.Array)
            {
                return (false, "Did not expect an array as the first or third parameter.");
            }

            if (children[1].Type != JTokenType.Array)
            {
                return (false, "Expected criteria array as the second parameter.");
            }

            var criteria = children[1].Children();
            if (criteria.Count() == 0)
            {
                return (false, "Expected entries in the criteria array.");
            }

            foreach(JToken jTokenCriteriaPair in criteria)
            {
                if(jTokenCriteriaPair.Type != JTokenType.Array)
                {
                    return (false, "Expected criteria array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Count() != 2)
                {
                    return (false, "Expected criteria array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Any(p => p.Type == JTokenType.Array))
                {
                    return (false, "Expected criteria array to contain only key-value pair arrays.");
                }
            }

            return (true, string.Empty);
        }
    }
}
