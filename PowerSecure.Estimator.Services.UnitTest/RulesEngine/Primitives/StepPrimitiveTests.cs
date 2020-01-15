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
    public class StepPrimitiveTests
    {

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

            Assert.AreEqual(3, (decimal)value, "Step did not work");
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
    }
}
