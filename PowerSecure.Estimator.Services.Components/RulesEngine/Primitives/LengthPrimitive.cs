using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class LengthPrimitive : IFunction
    {
        public string Name => "len";

        public object Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            object p = parameters[0].ToResolvedParameter();
            switch (p)
            {
                case object[] objs:
                    {
                        return (decimal)objs.Length;
                    }
                default:
                    {
                        return (decimal)p.ToRawString().Length;
                    }
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
