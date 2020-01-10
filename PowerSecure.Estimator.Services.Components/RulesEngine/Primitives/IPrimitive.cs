using System;
using System.Collections.Generic;
using System.Text;

namespace powersecure_instruction_set_engine.Primitives
{
    public interface IPrimitive
    {
        string Name { get; }

        int ParameterCount { get; }

        Decimal Invoke(params object[] parameters);
    }
}
