// 3 parameters - a value, an array of 2-element arrays, each with a value to match and a value to return if matched, and a value to return if there are no matches.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class SwitchPrimitive : IFunction
    {
        public string Name => "switch";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            if(parameters[0] == null || parameters[2] == null)
            {
                return null;
            }
            var value = parameters[0].ToComparable();
            var cases = parameters[1].ToObjectArray();

            if(value == null)
            {
                return null;
            }

            foreach(object obj in cases)
            {
                var pair = obj.ToObjectArray();
                if(pair[0] == null || pair[1] == null)
                {
                    return null;
                }

                if(value.CompareTo(pair[0].ToComparable()) == 0)
                {
                    return pair[1].ToResolvedParameter();
                }
            }

            return parameters[2].ToResolvedParameter();
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            JToken[] children = jToken.Children().ToArray();

            if (children.Count() != 3)
            {
                return (false, $"Expected a parameter array of length 3, got the following: {jToken.Children().Count()}");
            }

            if (children[0].Type == JTokenType.Array || children[2].Type == JTokenType.Array)
            {
                return (false, "Did not expect an array as the first or third parameter.");
            }

            if (children[1].Type != JTokenType.Array)
            {
                return (false, "Expected cases array as the second parameter.");
            }

            var criteria = children[1].Children();
            if (criteria.Count() == 0)
            {
                return (false, "Expected entries in the cases array.");
            }

            foreach (JToken jTokenCriteriaPair in criteria)
            {
                if (jTokenCriteriaPair.Type != JTokenType.Array)
                {
                    return (false, "Expected cases array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Count() != 2)
                {
                    return (false, "Expected cases array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Any(p => p.Type == JTokenType.Array))
                {
                    return (false, "Expected cases array to contain only key-value pair arrays.");
                }
            }

            return (true, string.Empty);
        }
    }
}
