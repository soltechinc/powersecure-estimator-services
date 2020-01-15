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
    public class PricePrimitiveTests
    {

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

            Assert.AreEqual(8, (decimal)value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invokeWithMargin()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6", "0.5" }, null);

            Assert.AreEqual(10, (decimal)value, "Price did not work");
        }

        [TestMethod]
        public void PricePrimitive_invokeWithApplyMargin()
        {
            var primitive = new PricePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6", "0.5", "0" }, null);

            Assert.AreEqual(8, (decimal)value, "Price did not work");
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
    }
}
