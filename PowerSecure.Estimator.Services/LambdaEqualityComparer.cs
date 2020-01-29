using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services
{
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        public LambdaEqualityComparer(Func<T,T, bool> equalsFunction, Func<T, int> hashCodeFunction)
        {
            EqualsFunction = equalsFunction;
            HashCodeFunction = hashCodeFunction;
        }

        public Func<T, T, bool> EqualsFunction { get; private set; }

        public Func<T, int> HashCodeFunction { get; private set; }

        public bool Equals(T x, T y)
        {
            return EqualsFunction(x,y);
        }

        public int GetHashCode(T obj)
        {
            return HashCodeFunction(obj);
        }
    }
}
