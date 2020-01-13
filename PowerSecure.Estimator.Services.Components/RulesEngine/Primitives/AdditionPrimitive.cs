using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public class AdditionPrimitive : IPrimitive
    {
        public string Name => "+";
    
        public decimal Invoke(params object[] parameters)
        {
            var decimals = Primitive.ConvertToDecimal(parameters);

            decimal value = 0;
            decimals.ForEach(p => value += p);
            return value;
        }

        public Tuple<bool, string> Validate(JToken jToken)
        {
            if (jToken.Children().Count() < 1)
            {
                return Tuple.Create(false, $"Expected a parameter array of length 1 or more, got the following: {jToken.Children().Count()}");
            }

            if (jToken.Children().Any(p => p.Type == JTokenType.Array))
            {
                return Tuple.Create(false, "Did not expect any arrays as parameters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
