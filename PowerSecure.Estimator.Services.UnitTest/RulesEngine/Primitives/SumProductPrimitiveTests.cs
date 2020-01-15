using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    [TestClass]
    public class SumProductPrimitiveTests
    {

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

            Assert.AreEqual(27, (decimal)value, "SumProduct did not work");
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
    }
}
