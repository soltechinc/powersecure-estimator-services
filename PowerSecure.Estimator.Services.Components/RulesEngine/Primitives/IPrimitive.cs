using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public interface IPrimitive
    {
        string Name { get; }

        Decimal Invoke(params object[] parameters);

        Tuple<bool, string> Validate(JToken jToken);
    }
}
