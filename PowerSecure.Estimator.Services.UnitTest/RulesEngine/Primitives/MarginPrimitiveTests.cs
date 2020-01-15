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
    public class MarginPrimitiveTests
    {

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
    }
}
