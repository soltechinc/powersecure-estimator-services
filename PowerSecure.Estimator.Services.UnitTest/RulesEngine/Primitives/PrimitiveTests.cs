using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    [TestClass]
    public class PrimitiveTests
    {
        [TestMethod]
        public void Load_bothMethodsMatch()
        {
            var primitivesFromT4 = Primitive.Load();

            var primitivesFromAssembly = Primitive.Load(typeof(Primitive).Assembly);

            Assert.AreNotEqual(0, primitivesFromT4.Count, "No primitives were loaded via T4-generated class");
            Assert.AreNotEqual(0, primitivesFromAssembly.Count, "No primitives were loaded via reflection");
            Assert.AreEqual(primitivesFromT4.Count, primitivesFromAssembly.Count, "Differing numbers of primitives loaded via reflection and T4-generated class");
        }

        [TestMethod]
        public void Load_onlyFindIsSpecial()
        {
            var primitives = Primitive.Load().Values.Where(p => !p.ResolveParameters).ToArray();

            Assert.AreEqual(1, primitives.Length, "More than one primitive that does not resolve parameters");
            Assert.IsTrue(primitives[0] is FindPrimitive, "The primitive that does not resolve parameters is not FindPrimitive");
        }

        [TestMethod]
        public void ConvertToDecimal_happyPath()
        {
            var decimals = Primitive.ConvertToDecimal("3","16.2", "0.14", "-2");

            Assert.AreEqual(4, decimals.Length, "Failed to convert all of the strings to decimals." );
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertToDecimal_errorString()
        {
            var decimals = Primitive.ConvertToDecimal("3", "not a decimal", "0.14", "-2");
        }

        [TestMethod]
        public void AdditionPrimitive_name()
        {
            var primitive = new AdditionPrimitive();

            Assert.AreEqual("+", primitive.Name, "Addition name changed");
        }

        [TestMethod]
        public void AdditionPrimitive_binaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(8, value, "Addition did not work");
        }

        [TestMethod]
        public void AdditionPrimitive_unaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(2, value, "Addition did not work");
        }

        [TestMethod]
        public void AdditionPrimitive_validateBinaryArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Addition binary arguments did not validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateUnaryArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Addition unary arguments did not validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateNoArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Addition zero arguments did validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateArrayArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Addition array arguments did validate");
        }

        [TestMethod]
        public void CheckPrimitive_name()
        {
            var primitive = new CheckPrimitive();

            Assert.AreEqual("if", primitive.Name, "Check name changed");
        }

        [TestMethod]
        public void CheckPrimitive_invokeWithTrue()
        {
            var primitive = new CheckPrimitive();

            var value = primitive.Invoke(new object[] { "1", "6", "-8" }, null);

            Assert.AreEqual(6, value, "Check (true value) did not work");
        }

        [TestMethod]
        public void CheckPrimitive_invokeWithFalse()
        {
            var primitive = new CheckPrimitive();

            var value = primitive.Invoke(new object[] { "0", "6", "-8" }, null);

            Assert.AreEqual(-8, value, "Check (false value) did not work");
        }

        [TestMethod]
        public void CheckPrimitive_validateTooFewArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsFalse(success, "Check arguments (too few) did validate");
        }

        [TestMethod]
        public void CheckPrimitive_validateTooManyArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', '8', '10' ]"));

            Assert.IsFalse(success, "Check arguments (too many) did validate");
        }

        [TestMethod]
        public void CheckPrimitive_validate()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '3', '6' ]"));

            Assert.IsTrue(success, "Check arguments did not validate");
        }

        [TestMethod]
        public void CheckPrimitive_validateArrayArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Check array arguments did validate");
        }

        [TestMethod]
        public void DivisionPrimitive_name()
        {
            var primitive = new DivisionPrimitive();

            Assert.AreEqual("/", primitive.Name, "Division name changed");
        }

        [TestMethod]
        public void DivisionPrimitive_binaryInvoke()
        {
            var primitive = new DivisionPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2" }, null);

            Assert.AreEqual(3, value, "Division did not work");
        }

        [TestMethod]
        public void DivisionPrimitive_ternaryInvoke()
        {
            var primitive = new DivisionPrimitive();

            var value = primitive.Invoke(new object[] { "30", "3", "2" }, null);

            Assert.AreEqual(5, value, "Division did not work");
        }

        [TestMethod]
        public void DivisionPrimitive_validateTooFewArguments()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Division arguments (too few) did validate");
        }

        [TestMethod]
        public void DivisionPrimitive_binaryValidate()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '6', '3' ]"));

            Assert.IsTrue(success, "Division arguments did not validate");
        }

        [TestMethod]
        public void DivisionPrimitive_ternaryValidate()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '6', '3', '2' ]"));

            Assert.IsTrue(success, "Division arguments did not validate");
        }

        [TestMethod]
        public void DivisionPrimitive_validateArrayArguments()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Division array arguments did validate");
        }

        [TestMethod]
        public void EqualPrimitive_name()
        {
            var primitive = new EqualPrimitive();

            Assert.AreEqual("=", primitive.Name, "Equal name changed");
        }

        [TestMethod]
        public void EqualPrimitive_invokeWithFalse()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2", "-3", "7" }, null);

            Assert.AreEqual(7, value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeWithTrue()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "2", "-3", "7" }, null);

            Assert.AreEqual(-3, value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_validate()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsTrue(success, "Equal arguments did not validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateTooFewArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "Equal arguments (too few) did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateTooManyArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Equal arguments (too many) did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateArrayArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2', '2' ]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_name()
        {
            var primitive = new FindPrimitive();

            Assert.AreEqual("find", primitive.Name, "Find name changed");
        }

        [TestMethod]
        public void FindPrimitive_validateTooFewArguments()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments (too few) did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateTooManyArguments()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments (too many) did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate1stArgumentArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate3rdArgumentArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', [] ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateNoArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateEmptyArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateOddArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '3' ], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '3', '4' ], '2' ]"));

            Assert.IsTrue(success, "Find arguments did not validate");
        }

        [TestMethod]
        public void MarginPrimitive_name()
        {
            var primitive = new MarginPrimitive();

            Assert.AreEqual("margin", primitive.Name, "Margin name changed");
        }

        [TestMethod]
        public void MarginPrimitive_invokeWithNegativePrice()
        {
            var primitive = new MarginPrimitive();

            var value = primitive.Invoke(new object[] { "-3", "2" }, null);

            Assert.AreEqual(0, value, "Margin did not work");
        }

        [TestMethod]
        public void MarginPrimitive_invokeWithZeroPrice()
        {
            var primitive = new MarginPrimitive();

            var value = primitive.Invoke(new object[] { "0", "2" }, null);

            Assert.AreEqual(0, value, "Margin did not work");
        }

        [TestMethod]
        public void MarginPrimitive_invoke()
        {
            var primitive = new MarginPrimitive();

            var value = primitive.Invoke(new object[] { "10", "6" }, null);

            Assert.AreEqual(0.4m, value, "Margin did not work");
        }

        [TestMethod]
        public void MarginPrimitive_invokeWithApplyMargin()
        {
            var primitive = new MarginPrimitive();

            var value = primitive.Invoke(new object[] { "10", "6", "2" }, null);

            Assert.AreEqual(0.8m, value, "Margin did not work");
        }

        [TestMethod]
        public void MarginPrimitive_validateTooFewArguments()
        {
            var primitive = new MarginPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Margin arguments (too few) did validate");
        }

        [TestMethod]
        public void MarginPrimitive_validate2Arguments()
        {
            var primitive = new MarginPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsTrue(success, "Margin arguments did not validate");
        }

        [TestMethod]
        public void MarginPrimitive_validate3Arguments()
        {
            var primitive = new MarginPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsTrue(success, "Margin arguments did not validate");
        }

        [TestMethod]
        public void MarginPrimitive_validateTooManyArguments()
        {
            var primitive = new MarginPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Margin arguments (too many) did validate");
        }

        [TestMethod]
        public void MarginPrimitive_validateArrayArguments()
        {
            var primitive = new MarginPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Margin array arguments did validate");
        }

        [TestMethod]
        public void MaxPrimitive_name()
        {
            var primitive = new MaxPrimitive();

            Assert.AreEqual("max", primitive.Name, "Max name changed");
        }

        [TestMethod]
        public void MaxPrimitive_invoke()
        {
            var primitive = new MaxPrimitive();

            var value = primitive.Invoke(new object[] { "-3", "2" }, null);

            Assert.AreEqual(2, value, "Max did not work");
        }

        [TestMethod]
        public void MaxPrimitive_validateTooFewArguments()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Max arguments (too few) did validate");
        }

        [TestMethod]
        public void MaxPrimitive_validate()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Max arguments did not validate");
        }

        [TestMethod]
        public void MaxPrimitive_validateArrayArguments()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Max array arguments did validate");
        }

        [TestMethod]
        public void MinPrimitive_name()
        {
            var primitive = new MinPrimitive();

            Assert.AreEqual("min", primitive.Name, "Min name changed");
        }

        [TestMethod]
        public void MinPrimitive_invoke()
        {
            var primitive = new MinPrimitive();

            var value = primitive.Invoke(new object[] { "-3", "2" }, null);

            Assert.AreEqual(-3, value, "Min did not work");
        }

        [TestMethod]
        public void MinPrimitive_validateTooFewArguments()
        {
            var primitive = new MinPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Min arguments (too few) did validate");
        }

        [TestMethod]
        public void MinPrimitive_validate()
        {
            var primitive = new MinPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Min arguments did not validate");
        }

        [TestMethod]
        public void MinPrimitive_validateArrayArguments()
        {
            var primitive = new MinPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Min array arguments did validate");
        }

        [TestMethod]
        public void MultiplicationPrimitive_name()
        {
            var primitive = new MultiplicationPrimitive();

            Assert.AreEqual("*", primitive.Name, "Multiplication name changed");
        }

        [TestMethod]
        public void MultiplicationPrimitive_invoke()
        {
            var primitive = new MultiplicationPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(12, value, "Multiplication did not work");
        }

        [TestMethod]
        public void MultiplicationPrimitive_validate()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Multiplication arguments did not validate");
        }

        [TestMethod]
        public void MultiplicationPrimitive_validateTooFewArguments()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Multiplication (too few) arguments did validate");
        }

        [TestMethod]
        public void MultiplicationPrimitive_validateArrayArguments()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Multiplication array arguments did validate");
        }

        [TestMethod]
        public void PricePrimitive_name()
        {
            var primitive = new PricePrimitive();

            Assert.AreEqual("price", primitive.Name, "Price name changed");
        }

        [TestMethod]
        public void PricePrimitive_invokeNegativePrice()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "-2", "6" }, null);

            Assert.AreEqual(0, value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invokeZeroPrice()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "0", "6" }, null);

            Assert.AreEqual(0, value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invoke()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(8, value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invokeWithMargin()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6", "0.5" }, null);

            Assert.AreEqual(10, value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invokeWithApplyMargin()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6", "0.5", "0" }, null);

            Assert.AreEqual(8, value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_validateTooFewArguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Price (too few) arguments did validate");
        }

        [TestMethod]
        public void PricePrimitive_validate2Arguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsTrue(success, "Price arguments did not validate");
        }

        [TestMethod]
        public void PricePrimitive_validate3Arguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsTrue(success, "Price arguments did not validate");
        }

        [TestMethod]
        public void PricePrimitive_validate4Arguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsTrue(success, "Price arguments did not validate");
        }

        [TestMethod]
        public void PricePrimitive_validateTooManyArguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Price (too many) arguments did validate");
        }

        [TestMethod]
        public void PricePrimitive_validateArrayArguments()
        {
            var primitive = new PricePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Price array arguments did validate");
        }

        [TestMethod]
        public void StepPrimitive_name()
        {
            var primitive = new StepPrimitive();

            Assert.AreEqual("++", primitive.Name, "Step name changed");
        }

        [TestMethod]
        public void StepPrimitive_invoke()
        {
            var primitive = new StepPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(3, value, "Step did not work");
        }

        [TestMethod]
        public void StepPrimitive_validateTooFewArguments()
        {
            var primitive = new StepPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Step (too few) arguments did validate");
        }

        [TestMethod]
        public void StepPrimitive_validate()
        {
            var primitive = new StepPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Step arguments did not validate");
        }

        [TestMethod]
        public void StepPrimitive_validateTooManyArguments()
        {
            var primitive = new StepPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "Step (too many) arguments did validate");
        }

        [TestMethod]
        public void StepPrimitive_validateArrayArguments()
        {
            var primitive = new StepPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Step array arguments did validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_name()
        {
            var primitive = new SubtractionPrimitive();

            Assert.AreEqual("-", primitive.Name, "Subtraction name changed");
        }

        [TestMethod]
        public void SubtractionPrimitive_binaryInvoke()
        {
            var primitive = new SubtractionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(-4, value, "Subtraction did not work");
        }

        [TestMethod]
        public void SubtractionPrimitive_unaryInvoke()
        {
            var primitive = new SubtractionPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(-2, value, "Subtraction did not work");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateBinaryArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Subtraction binary arguments did not validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateUnaryArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Subtraction unary arguments did not validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateNoArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Subtraction zero arguments did validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateArrayArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Subtraction array arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_name()
        {
            var primitive = new SumProductPrimitive();

            Assert.AreEqual("sumproduct", primitive.Name, "SumProduct name changed");
        }

        [TestMethod]
        public void SumProductPrimitive_invoke()
        {
            var primitive = new SumProductPrimitive();

            var value = primitive.Invoke(new object[] { new string[] { "2", "5" }, new string[] { "6", "3" } }, null);

            Assert.AreEqual(27, value, "SumProduct did not work");
        }

        [TestMethod]
        public void SumProductPrimitive_validateTooFewArguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "SumProduct (too few) arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validateNotArray1Arguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [] ]"));

            Assert.IsFalse(success, "SumProduct arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validateNotArray2Arguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "SumProduct arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validateTooManyArguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "SumProduct (too many) arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validateNonequalSizeArrayArguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], ['1'] ]"));

            Assert.IsFalse(success, "SumProduct arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validateEmptyArrayArguments()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], [] ]"));

            Assert.IsFalse(success, "SumProduct arguments did validate");
        }

        [TestMethod]
        public void SumProductPrimitive_validate()
        {
            var primitive = new SumProductPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['3'], ['4'] ]"));

            Assert.IsTrue(success, "SumProduct arguments did not validate");
        }

        [TestMethod]
        public void ThresholdPrimitive_name()
        {
            var primitive = new ThresholdPrimitive();

            Assert.AreEqual("threshold", primitive.Name, "Threshold name changed");
        }

        [TestMethod]
        public void ThresholdPrimitive_invokeThresholdMet()
        {
            var primitive = new ThresholdPrimitive();

            var value = primitive.Invoke(new object[] { "3", "3", "-4", "19" }, null);

            Assert.AreEqual(-4, value, "Threshold did not work");
        }

        [TestMethod]
        public void ThresholdPrimitive_invokeThresholdNotMet()
        {
            var primitive = new ThresholdPrimitive();

            var value = primitive.Invoke(new object[] { "2", "3", "-4", "19" }, null);

            Assert.AreEqual(19, value, "Threshold did not work");
        }

        [TestMethod]
        public void ThresholdPrimitive_validateTooFewArguments()
        {
            var primitive = new ThresholdPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "Threshold (too few) arguments did validate");
        }

        [TestMethod]
        public void ThresholdPrimitive_validateTooManyArguments()
        {
            var primitive = new ThresholdPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Threshold (too many) arguments did validate");
        }

        [TestMethod]
        public void ThresholdPrimitive_validateArrayArguments()
        {
            var primitive = new ThresholdPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2', '2' ]"));

            Assert.IsFalse(success, "Threshold array arguments did validate");
        }

        [TestMethod]
        public void ThresholdPrimitive_validate()
        {
            var primitive = new ThresholdPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '3', '2', '2', '2' ]"));

            Assert.IsTrue(success, "Threshold arguments did not validate");
        }

        [TestMethod]
        public void ZeroPrimitive_name()
        {
            var primitive = new ZeroPrimitive();

            Assert.AreEqual("zero", primitive.Name, "Zero name changed");
        }

        [TestMethod]
        public void ZeroPrimitive_invokeNegative()
        {
            var primitive = new ZeroPrimitive();

            var value = primitive.Invoke(new object[] { "-3" }, null);

            Assert.AreEqual(0, value, "Zero did not work");
        }

        [TestMethod]
        public void ZeroPrimitive_invokeZero()
        {
            var primitive = new ZeroPrimitive();

            var value = primitive.Invoke(new object[] { "0" }, null);

            Assert.AreEqual(0, value, "Zero did not work");
        }

        [TestMethod]
        public void ZeroPrimitive_invokePositive()
        {
            var primitive = new ZeroPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(2, value, "Zero did not work");
        }

        [TestMethod]
        public void ZeroPrimitive_validateTooFewArguments()
        {
            var primitive = new ZeroPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[  ]"));

            Assert.IsFalse(success, "Zero (too few) arguments did validate");
        }

        [TestMethod]
        public void ZeroPrimitive_validateTooManyArguments()
        {
            var primitive = new ZeroPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "Zero (too many) arguments did validate");
        }

        [TestMethod]
        public void ZeroPrimitive_validateArrayArguments()
        {
            var primitive = new ZeroPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Zero array arguments did validate");
        }

        [TestMethod]
        public void ZeroPrimitive_validate()
        {
            var primitive = new ZeroPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '3' ]"));

            Assert.IsTrue(success, "Zero arguments did not validate");
        }
    }
}
