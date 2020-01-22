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
    public class NotEqualPrimitiveTests
    {

        [TestMethod]
        public void NotEqualPrimitive_name()
        {
            var primitive = new NotEqualPrimitive();

            Assert.AreEqual("!=", primitive.Name, "NotEqual name changed");
        }

        [TestMethod]
        public void NotEqualPrimitive_invokeWithTrue() // true
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2", "trueValue", "falseValue" }, null);

            Assert.AreEqual("trueValue", value, "NotEqual did work");
        }

        [TestMethod]
        public void NotEqualPrimitive_invokeWithFalse()
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "2", "trueValue", "falseValue" }, null);

            Assert.AreEqual("falseValue", value, "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_validate()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsTrue(success, "NotEqual arguments did not validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateTooFewArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "NotEqual arguments (too few) did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateTooManyArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "NotEqual arguments (too many) did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateArrayArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2', '2' ]"));

            Assert.IsFalse(success, "NotEqual array arguments did validate");
        }
    }
}
