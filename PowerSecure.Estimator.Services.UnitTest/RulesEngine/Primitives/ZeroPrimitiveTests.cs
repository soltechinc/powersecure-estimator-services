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
    public class ZeroPrimitiveTests
    {

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

            Assert.AreEqual(0, (decimal)value, "Zero did not work");
        }

        [TestMethod]
        public void ZeroPrimitive_invokeZero()
        {
            var primitive = new ZeroPrimitive();

            var value = primitive.Invoke(new object[] { "0" }, null);

            Assert.AreEqual(0, (decimal)value, "Zero did not work");
        }

        [TestMethod]
        public void ZeroPrimitive_invokePositive()
        {
            var primitive = new ZeroPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(2, (decimal)value, "Zero did not work");
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
