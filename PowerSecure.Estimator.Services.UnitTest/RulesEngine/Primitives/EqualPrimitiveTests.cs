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
    public class EqualPrimitiveTests
    {

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

            var value = primitive.Invoke(new object[] { "6", "2", "trueValue", "falseValue" }, null);

            Assert.AreEqual("falseValue", value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeWithTrue()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "2", "trueValue", "falseValue" }, null);

            Assert.AreEqual("trueValue", value, "Equal did not work");
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
    }
}
