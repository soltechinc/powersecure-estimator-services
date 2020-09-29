using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Collections.Generic;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class FilterPrimitive : IFunction
    {
        public string Name => "filter";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var searchString = parameters[0].ToStringLiteral();
            var up = (UnresolvedParameter)parameters[1];
            var objs = parameters[2].ToObjectArray();

            switch (up.Token.Type)
            {
                case JTokenType.Array:
                    {
                        var list = new List<object>();
                        var filterArray = parameters[1].ToObjectArray().Select(o => (UnresolvedParameter)o).ToArray();
                        foreach (var objArray in objs.Select(o => o.ToObjectArray()))
                        {
                            if (objArray.Length < filterArray.Length)
                            {
                                continue;
                            }

                            bool add = true;
                            var resolvedObjs = objArray.Select(o => o.ToResolvedParameter()).ToArray();
                            for (int i = 0; i < filterArray.Length; ++i)
                            {
                                if (resolvedObjs[i].ToComparable().CompareTo(filterArray[i].ToInstructionSet(resolvedObjs[i], searchString).ToComparable()) != 0)
                                {
                                    add = false;
                                }
                            }

                            if (add)
                            {
                                list.Add(resolvedObjs);
                            }
                        }
                        return list.ToArray();
                    }
                default:
                    {
                        return objs.Select(o => o.ToResolvedParameter()).Where(o => o.ToComparable().CompareTo(up.ToInstructionSet(o, searchString).ToComparable()) == 0).ToArray();
                    }
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 2)
            {
                return (false, $"Expected a parameter array of length 2, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
