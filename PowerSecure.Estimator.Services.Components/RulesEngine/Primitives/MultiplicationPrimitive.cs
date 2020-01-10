using System;
using System.Collections.Generic;
using System.Text;

namespace powersecure_instruction_set_engine.Primitives
{
    public class MultiplicationPrimitive : IPrimitive
    {
        public string Name => "*";

        public int ParameterCount => 2;

        public decimal Invoke(params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
