using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Collections.Generic;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class FlattenPrimitive : IFunction
    {
        public string Name => "flatten";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            return FlattenEnumerator(parameters).ToArray();
        }

        private IEnumerable<object> FlattenEnumerator(object[] parameters)
        {
            foreach (var obj in parameters)
            {
                if (obj == null)
                {
                    yield return null;
                    continue;
                }

                var resolvedObj = obj.ToResolvedParameter();
                switch (resolvedObj)
                {
                    case object[] objs:
                        {
                            foreach (var subObj in FlattenEnumerator(objs))
                            {
                                yield return subObj;
                            }
                        }
                        break;
                    default:
                        {
                            yield return resolvedObj;
                        }
                        break;
                }
            }
        }

        public (bool Success, string Message) Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return (false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            return (true, string.Empty);
        }
    }
}
