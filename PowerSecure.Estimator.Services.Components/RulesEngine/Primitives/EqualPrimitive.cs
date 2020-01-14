// 4 parameters. If the first two parameters are equal, returns parameter 3. Otherwise, returns parameter 4.
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class EqualPrimitive : IPrimitive
    {
        public string Name => "=";

        public bool ResolveParameters => true;

        public decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository)
        {
            var decimals = Primitive.ConvertToDecimal(parameters);

            return decimals[0] == decimals[1] ? decimals[2] : decimals[3];
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() != 4)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 4, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
