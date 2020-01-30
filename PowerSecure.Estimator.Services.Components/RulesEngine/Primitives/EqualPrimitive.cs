// 2 or more parameters, or 1 parameter if it is an array.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class EqualPrimitive : IPrimitive
    {
        public string Name => "=";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            switch (parameters.Length)
            {
                case 1:
                    return CheckEquality(parameters[0].ToObjectArray());
                case 2:
                    {
                        var first = parameters[0].ToComparable();
                        var second = parameters[1].ToResolvedParameter();

                        if (second is object[] objects)
                        {
                            return objects.Select(o => (object)(first.CompareTo(o.ToComparable()) == 0)).ToArray();
                        }

                        return first.CompareTo(second.ToComparable()) == 0;
                    }
                default:
                    return CheckEquality(parameters);
            }
        }

        private static bool CheckEquality(IEnumerable<object> objects)
        {
            IComparable previous = null;
            foreach (var compare in objects.ToComparable())
            {
                if (previous != null)
                {
                    if (compare.CompareTo(previous) != 0)
                    {
                        return false;
                    }
                }

                previous = compare;
            }

            return true;
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                if (jToken.Children().Count() > 2)
                {
                    return (false, "Did not expect any arrays as parameters.");
                }

                if (jToken.Children().Count() == 1)
                {
                    var child = jToken.Children().First();
                    if (child.Children().Count() < 2)
                    {
                        return (false, $"Expected an array of length 2 or more as a parameter, got the following: {child.Children().Count()}");
                    }

                    if (child.Children().Any(p => p.Type == JTokenType.Array))
                    {
                        return (false, "Did not expect any nested arrays as parameters.");
                    }
                }
                else //jToken.Children().Count() == 2
                {
                    var firstChild = jToken.Children().First();

                    if (firstChild.Type == JTokenType.Array)
                    {
                        return (false, "Did not expect a nested arrays as the first parameter.");
                    }

                    var lastChild = jToken.Children().Last();
                    if (lastChild.Children().Count() < 1)
                    {
                        return (false, $"Expected an array of length 1 or more as a parameter, got the following: {lastChild.Children().Count()}");
                    }

                    if (lastChild.Children().Any(p => p.Type == JTokenType.Array))
                    {
                        return (false, "Did not expect any nested arrays as parameters.");
                    }
                }
            }
            else
            {
                if (jToken.Children().Count() < 2)
                {
                    return (false, $"Expected a parameter array of length 2 or more, got the following: {jToken.Children().Count()}");
                }
            }

            return (true, string.Empty);
        }
    }
}
