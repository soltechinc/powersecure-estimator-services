using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.Calculator
{
    public class Calculator<T> {
        AbstractCalculator<T> _myCalculator;


        public Calculator(AbstractCalculator<T> myCalculator) {
            this._myCalculator = myCalculator;
        }

        public T Sum(T a, T b) {
            return _myCalculator.Sum(a, b);
        }

        public T Difference(T a, T b) {
            return _myCalculator.Difference(a, b);
        }

        public T Product(T a, T b) {
            return _myCalculator.Product(a, b);
        }

        public T Quotient(T a, T b) {
            return _myCalculator.Quotient(a, b);
        }

        public T Max(T a, T b) {
            return _myCalculator.Max(a, b);
        }

        public T Min(T a, T b) {
            return _myCalculator.Min(a, b);
        }

        public T Sum(T[] items) {
            dynamic sum = 0;

            for (int i = 0; i < items.Length; i++) {
                sum = _myCalculator.Sum(sum, items[i]);
            }
            return sum;
        }


        public T Product(T[] items) {
            dynamic mul = 1;

            for (int i = 0; i < items.Length; i++) {
                mul = _myCalculator.Product(mul, items[i]);
            }
            return mul;
        }

        public T Ceiling(T a, T b) {
            return _myCalculator.Ceiling(a, b);
        }

        public T SumProduct(T[] values, T[] factors) {
            return _myCalculator.SumProduct(values, factors);
        }

        public T IsZero(T paramA) {
            return _myCalculator.IsZero(paramA);
        }

        public T RoundUp(T value, T numberOfDecimalPlaces) {
            return _myCalculator.RoundUp(value, numberOfDecimalPlaces);
        }
    }
}