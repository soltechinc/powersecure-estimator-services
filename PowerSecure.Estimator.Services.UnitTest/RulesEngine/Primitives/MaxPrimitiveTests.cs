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
    public class MaxPrimitiveTests
    {

        [TestMethod]
        public void MaxPrimitive_name()
        {
            var primitive = new MaxPrimitive();

            Assert.AreEqual("max", primitive.Name, "Max name changed");
        }

        [TestMethod]
        public void MaxPrimitive_invoke()
        {
            var primitive = new MaxPrimitive();

            var value = primitive.Invoke(new object[] { "-3", "2" }, null);

            Assert.AreEqual(2, (decimal)value, "Max did not work");
        }

        [TestMethod]
        public void MaxPrimitive_unaryArraynvoke()
        {
            var primitive = new MaxPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-3", "2" } }, null);

            Assert.AreEqual(2, (decimal)value, "Max did not work");
        }

        [TestMethod]
        public void MaxPrimitive_validateTooFewArguments()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Max arguments (too few) did validate");
        }

        [TestMethod]
        public void MaxPrimitive_validate()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Max arguments did not validate");
        }

        [TestMethod]
        public void MaxPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Max array arguments did validate");
        }

        [TestMethod]
        public void MaxPrimitive_validateArrayArguments()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['3', '2'] ]"));

            Assert.IsTrue(success, "Max array arguments did not validate");
        }

        [TestMethod]
        public void MaxPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new MaxPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Max array arguments did validate");
        }
    }
}
