using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Calculator {
    public abstract class AbstractCalculator<T> {
        public abstract T Sum(T paramA, T paramB);
        public abstract T Difference(T paramA, T paramB);
        public abstract T Product(T paramA, T paramB);
        public abstract T Quotient(T paramA, T paramB);
        public abstract T Max(T paramA, T paramB);
        public abstract T Min(T paramA, T paramB);
        public abstract T Ceil(T paramA, T paramB);
        public abstract T SumProduct(T[] values, T[] factors);
    }
}
