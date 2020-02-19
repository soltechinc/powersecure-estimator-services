﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.Calculator
{
    public class IntCalculator : AbstractCalculator<int> {
        public override int Sum(int paramA, int paramB) {
            return paramA + paramB;
        }

        public override int Difference(int paramA, int paramB) {
            return paramA - paramB;
        }

        public override int Product(int paramA, int paramB) {

            return paramA * paramB;
        }

        public override int Quotient(int paramA, int paramB) {
            return paramA / paramB;
        }

        public override int Max(int paramA, int paramB) {
            return (paramA > paramB) ? paramA : paramB;
        }
        
        public override int Min(int paramA, int paramB) {
            return (paramA < paramB) ? paramA : paramB;
        }

        public override int Ceiling(int paramA, int paramB) {
            bool overZero = (paramA > 0 && paramB > 0) ? true : false;
            if (overZero && paramA > paramB) {
                var quotient = (int)Math.Floor((double)Quotient(paramA, paramB)) + 1;
                return Product(quotient, paramB);
            } else if (overZero && paramB > paramA) {
                return paramB;
            } else {
                return 0;
            }
        }

        private int AddArrayValues(int[] arr) {
            int results = 0;
            try {
                if (arr.Length > 1) {
                    for (int k = 0; arr.Length > k; k++) {
                        results = Sum(0, arr[k]) + results;
                    }
                } else if(arr.Length == 1) {
                    results = arr[0];
                } else {
                    throw new Exception("Array Length is not valid");
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return results;
        }

        public override int SumProduct(int[] values, int[]factors) {
            int prodValues;
            int results = 0;
            try {
                if (values.Length == factors.Length) {
                    for (int i = 0; values.Length > i; i++) {
                        int[] prodResultsArr = new int[values.Length];
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
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return results;
        }

        public override int IsZero(int paramA) {
            return (paramA < 0) ? 0 : paramA;
        }

        public override int RoundUp(int value, int numberOfDecimalPlaces) {
            throw new NotImplementedException();
        }
    }
}