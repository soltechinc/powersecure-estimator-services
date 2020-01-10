using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public interface IPrimitive
    {
        string Name { get; }

        int ParameterCount { get; }

        Decimal Invoke(params object[] parameters);
    }
}
