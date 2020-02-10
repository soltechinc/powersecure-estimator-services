using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerSecure.Estimator.Services.Components.Calculator
{
    public class DoubleCalculator : AbstractCalculator<double> {
        public override double Sum(double paramA, double paramB) {
            return paramA + paramB;
        }

        public override double Difference(double paramA, double paramB) {
            return paramA - paramB;
        }

        public override double Product(double paramA, double paramB) {

            return paramA * paramB;
        }

        public override double Quotient(double paramA, double paramB) {
            return paramA / paramB;
        }

        public override double Max(double paramA, double paramB) {
            return (paramA > paramB) ? paramA : paramB;
        }

        public override double Min(double paramA, double paramB) {
            return (paramA < paramB) ? paramA : paramB;
        }

        public override double Ceil(double paramA, double paramB) {
            double results = 0;
            try {
                if (paramA > 0 && paramB > 0 && paramA > paramB) {
                    double quotient = Quotient(paramA, paramB) + 1;
                    results = Product(quotient, paramB);
                }
                return results;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return results;
            }
        }

        private double AddArrayValues(double[] arr) {
            double results = 0;
            try {
                if (arr.Length > 1) {
                    for (int k = 0; arr.Length > k; k++) {
                        results = Sum(0, arr[k]) + results;
                    }
                } else if (arr.Length == 1) {
                    results = arr[0];
                } else {
                    throw new Exception("Array Length is not valid");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return results;
        }

        public override double SumProduct(double[] values, double[] factors) {
            double prodValues;
            double results = 0;
            try {
                if (values.Length == factors.Length) {
                    for (int i = 0; values.Length > i; i++) {
                        double[] prodResultsArr = new double[values.Length];
                        for (int j = 0; factors.Length > j; j++) {
                            prodValues = Product(values[i], factors[j]);
                            prodResultsArr[i] = prodValues;
                            i++;
                            if (values.Length == i) {
                                results = AddArrayValues(prodResultsArr);
                            }
                        }
                    }
                } else {
                    throw new Exception("Array Length is not valid");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return results;
        }

        public override double IsZero(double paramA) {
            return (paramA < 0) ? 0 : paramA;
        }


        private double ToIncrementNumber(int digitsToChange, bool isGreaterThanZero) {
            double incrementByValue = 1;
            for (var i = digitsToChange; i > 0; i--) {
                incrementByValue = (isGreaterThanZero) ? incrementByValue *= 10 : incrementByValue /= 10;
            }
            return incrementByValue;
        }

        private string ReplaceValueWithZero(string str, int digitsToChange) {
            char[] charArr = str.ToCharArray();
            int charArrLength = charArr.Length;
            for (int i = charArrLength - 1; i >= charArrLength - digitsToChange; i--) {
                charArr[i] = '0';
            }            
            return new string(charArr);
        }
        public override double RoundUp(double value, double numberOfDecimalPlaces) {
            int digitsToChange = (int)Math.Abs(numberOfDecimalPlaces);
            bool isGreaterThanZero = (numberOfDecimalPlaces < 0) ? true : false;
            if (numberOfDecimalPlaces < 0) {
                int lengthOfValue = ((int)Math.Abs(value)).ToString().Length;
                if (lengthOfValue > digitsToChange) {
                    double incrementedValue = ToIncrementNumber(digitsToChange, isGreaterThanZero);
                    value = Math.Floor(value + incrementedValue);
                    string strVal = ReplaceValueWithZero(value.ToString(), digitsToChange);
                    value = Convert.ToDouble(strVal);
                } else {
                    value = ToIncrementNumber(digitsToChange, isGreaterThanZero);
                }
            } else if(numberOfDecimalPlaces > 0) {
                double incrementedValue = ToIncrementNumber(digitsToChange, isGreaterThanZero);
                value = value + incrementedValue;
                value = Math.Round(value, (int)numberOfDecimalPlaces, MidpointRounding.AwayFromZero);
            } else {
                value = Math.Ceiling(value);
            }
            return value;
        }
    }
}