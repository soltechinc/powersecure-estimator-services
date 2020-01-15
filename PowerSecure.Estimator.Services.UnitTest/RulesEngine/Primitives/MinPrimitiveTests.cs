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
    public class MinPrimitiveTests
    {

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

            Assert.AreEqual(-3, (decimal)value, "Min did not work");
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
    }
}
