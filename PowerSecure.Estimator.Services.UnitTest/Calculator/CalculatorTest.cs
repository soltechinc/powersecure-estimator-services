using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components;
using PowerSecure.Estimator.Services.Components.Calculator;

namespace PowerSecure.Estimator.Services.UnitTest.Calculator {
    [TestClass]
    public class CalculatorTest {

        [TestMethod]
        public void CheckDoubleIsZero() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = -2;
            var value = cal.IsZero(a);
            var expectedResult = 0;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestSmallValueWithPositiveNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 5.12423;
            var b = 3;
            var value = cal.RoundUp(a, b);
            var expectedResult = 5.125;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestLargeValueWithPositiveNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 1502.12423;
            var b = 4;
            var value = cal.RoundUp(a, b);
            var expectedResult = 1502.1243;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestSmallValueWithNegativeNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 5.12423;
            var b = -4;
            var value = cal.RoundUp(a, b);
            var expectedResult = 10000;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestLargeValueWithNegativeNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 1502.12423;
            var b = -4;
            var value = cal.RoundUp(a, b);
            var expectedResult = 10000;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestGiantValueWithLargeNegativeNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 895489.323434;
            var b = -4;
            var value = cal.RoundUp(a, b);
            var expectedResult = 900000;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void TestLargerValueWithNegativeNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 23242.3898;
            var b = -4;
            var value = cal.RoundUp(a, b);
            var expectedResult = 30000;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestLargerValueWithNegativeNumberOfDecimalsExampleTwo() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 23242.378;
            var b = -3;
            var value = cal.RoundUp(a, b);
            var expectedResult = 24000;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void TestSmallValueWithZeroNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 5.12423;
            var b = 0;
            var value = cal.RoundUp(a, b);
            var expectedResult = 6;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void TestLargeValueWithZeroNumberOfDecimals() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 1502.12423;
            var b = 0;
            var value = cal.RoundUp(a, b);
            var expectedResult = 1503;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckDoubleSumProductErrMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var values = new double[] { 1, 2, 3 };
            var factors = new double[] { 1, 2 };
            var value = cal.SumProduct(values, factors);
            var expectResults = 0;
            Assert.AreEqual(value, expectResults);
        }

        [TestMethod]
        public void CheckDoubleSumProductMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var values = new double[] { 5.5, 12.05 };
            var factors = new double[] { 11.32, 4.13 };
            var value = cal.SumProduct(values, factors);
            var expectResults = 112.0265;
            Assert.AreEqual(value, expectResults);
        }


        [TestMethod]
        public void CheckIntSumProductErrMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var values = new int[] { 1, 2, 3 };
            var factors = new int[] { 1, 2 };
            var value = cal.SumProduct(values, factors);
            var expectResults = 0;
            Assert.AreEqual(value, expectResults);
        }

        [TestMethod]
        public void CheckIntSumProductMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var values = new int[] { 5, 12 };
            var factors = new int[] { 11, 4 };
            var value = cal.SumProduct(values, factors);
            var expectResults = 103;
            Assert.AreEqual(value, expectResults);
        }

        [TestMethod]
        public void CheckDoubleCeilMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 37;
            var b = 5;
            var value = cal.Ceil(a, b);
            var expectedResult = 42;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckIntCeilMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 37;
            var b = 5;
            var value = cal.Ceil(a, b);
            var expectedResult = 40;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckArrayIntProductMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var arr = new int[] { 1, 2, 4, 5 };
            var value = cal.Product(arr);
            var expectedResult = 40;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckArrayIntSumMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var arr = new int[] { 1, 2, 4, 5 };
            var value = cal.Sum(arr);
            var expectedResult = 12;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckIntMax() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 5;
            var b = 4;
            var value = cal.Max(a, b);
            var expectedResult = a;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckDoubleMax() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 5;
            var b = 4;
            var value = cal.Max(a, b);
            var expectedResult = a;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckIntMin() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 5;
            var b = 4;
            var value = cal.Min(a, b);
            var expectedResult = b;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckDoubleMin() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 6.5;
            var b = 6.4;
            var value = cal.Min(a, b);
            var expectedResult = b;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckDoubleQuotientMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 5.54;
            var b = 2.5;
            var value = cal.Quotient(a, b);
            var expectedResult = 2.216;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckIntQuotientMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 5;
            var b = 2;
            var value = cal.Quotient(a, b);
            var expectedResult = 2;
            Assert.AreEqual(value, expectedResult);
        }



        [TestMethod]
        public void CheckDoubleProductMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 1.5;
            var b = 2.5;
            var value = cal.Product(a, b);
            var expectedResult = 3.75;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckIntProductMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 4;
            var b = 2;
            var value = cal.Product(a, b);
            var expectedResult = 8;
            Assert.AreEqual(value, expectedResult);
        }


        [TestMethod]
        public void CheckDoubleDifferenceMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 5;
            var b = 2.5;
            var value = cal.Difference(a, b);
            var expectedResult = 2.5;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckIntDifferenceMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 5;
            var b = -2;
            var value = cal.Difference(a, b);
            var expectedResult = 7;
            Assert.AreEqual(value, expectedResult);
        }



        [TestMethod]
        public void CheckDoubleAdditionMethod() {
            Calculator<double> cal = new Calculator<double>(new DoubleCalculator());
            var a = 1.5;
            var b = 2.5;
            var value = cal.Sum(a, b);
            var expectedResult = 4;
            Assert.AreEqual(value, expectedResult);
        }

        [TestMethod]
        public void CheckIntAdditionMethod() {
            Calculator<int> cal = new Calculator<int>(new IntCalculator());
            var a = 1;
            var b = 2;
            var value = cal.Sum(a, b);
            var expectedResult = 3;
            Assert.AreEqual(value, expectedResult);
        }
    }
}