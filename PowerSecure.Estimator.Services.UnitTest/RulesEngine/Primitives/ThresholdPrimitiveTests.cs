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
    public class ThresholdPrimitiveTests
    {

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

            var value = primitive.Invoke(new object[] { "3", "3", "trueValue", "falseValue" }, null);

            Assert.AreEqual("trueValue", value, "Threshold did not work");
        }

        [TestMethod]
        public void ThresholdPrimitive_invokeThresholdNotMet()
        {
            var primitive = new ThresholdPrimitive();

            var value = primitive.Invoke(new object[] { "2", "3", "trueValue", "falseValue" }, null);

            Assert.AreEqual("falseValue", value, "Threshold did not work");
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
    }
}
