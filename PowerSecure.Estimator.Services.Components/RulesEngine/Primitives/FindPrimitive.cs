﻿// 3 parameters. Parameter 1 is the data set name, Parmeter 2 is the criteria array, Parameter 3 is the return field name
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
            var list = new List<KeyValuePair<string, string>>();
            var criteria = (object[])parameters[1];
            foreach(object obj in criteria)
            {
                var pair = (object[])obj;
                list.Add(new KeyValuePair<string,string>(pair[0].ToString(), pair[1].ToString()));
            }
            return referenceDataRepository.Lookup((string)parameters[0], list.ToArray(), (string)parameters[2]);
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

            var criteria = children[1].Children();
            if (criteria.Count() == 0)
            {
                return Tuple.Create(false, "Expected entries in the criteria array.");
            }

            foreach(JToken jTokenCriteriaPair in criteria)
            {
                if(jTokenCriteriaPair.Type != JTokenType.Array)
                {
                    return Tuple.Create(false, "Expected criteria array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Count() != 2)
                {
                    return Tuple.Create(false, "Expected criteria array to contain only key-value pair arrays.");
                }

                if (jTokenCriteriaPair.Children().Any(p => p.Type == JTokenType.Array))
                {
                    return Tuple.Create(false, "Expected criteria array to contain only key-value pair arrays.");
                }
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
