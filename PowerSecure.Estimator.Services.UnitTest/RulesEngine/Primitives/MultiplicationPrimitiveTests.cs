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
    public class MultiplicationPrimitiveTests
    {

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

            Assert.AreEqual(12, (decimal)value, "Multiplication did not work");
        }

        [TestMethod]
        public void MultiplicationPrimitive_unaryArrayInvoke()
        {
            var primitive = new MultiplicationPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "6" } }, null);

            Assert.AreEqual(12, (decimal)value, "Multiplication did not work");
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
        public void MultiplicationPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Multiplication array arguments did validate");
        }

        [TestMethod]
        public void MultiplicationPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Multiplication array arguments did validate");
        }

        [TestMethod]
        public void MultiplicationPrimitive_validateArrayArguments()
        {
            var primitive = new MultiplicationPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "Multiplication array arguments did not validate");
        }
    }
}
