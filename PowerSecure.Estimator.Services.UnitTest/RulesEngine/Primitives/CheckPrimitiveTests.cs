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
    public class CheckPrimitiveTests
    {

        [TestMethod]
        public void CheckPrimitive_name()
        {
            var primitive = new CheckPrimitive();

            Assert.AreEqual("if", primitive.Name, "Check name changed");
        }

        [TestMethod]
        public void CheckPrimitive_invokeWithTrue()
        {
            var primitive = new CheckPrimitive();

            var value = primitive.Invoke(new object[] { true, 6m, -8m }, null);

            Assert.AreEqual(6, (decimal)value, "Check (true value) did not work");
        }

        [TestMethod]
        public void CheckPrimitive_invokeWithFalse()
        {
            var primitive = new CheckPrimitive();

            var value = primitive.Invoke(new object[] { false, 6m, -8m }, null);

            Assert.AreEqual(-8, (decimal)value, "Check (false value) did not work");
        }

        [TestMethod]
        public void CheckPrimitive_validateTooFewArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsFalse(success, "Check arguments (too few) did validate");
        }

        [TestMethod]
        public void CheckPrimitive_validateTooManyArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', '8', '10' ]"));

            Assert.IsFalse(success, "Check arguments (too many) did validate");
        }

        [TestMethod]
        public void CheckPrimitive_validate()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '3', '6' ]"));

            Assert.IsTrue(success, "Check arguments did not validate");
        }

        [TestMethod]
        public void CheckPrimitive_validateArrayArguments()
        {
            var primitive = new CheckPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Check array arguments did validate");
        }
    }
}
