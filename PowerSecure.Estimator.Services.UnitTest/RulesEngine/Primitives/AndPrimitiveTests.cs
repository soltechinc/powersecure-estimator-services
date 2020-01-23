using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class AndPrimitiveTests
    {
        [TestMethod]
        public void AndPrimitive_name()
        {
            var primitive = new AndPrimitive();

            Assert.AreEqual("and", primitive.Name, "And name changed");
        }

        [TestMethod]
        public void AndPrimitive_invokeTrue()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { true, true }, null);

            Assert.AreEqual(true, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_invokeFalse()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { true, false }, null);

            Assert.AreEqual(false, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { true, true } }, null);

            Assert.AreEqual(true, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { true, false } }, null);

            Assert.AreEqual(false, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { true, true, true, true } }, null);

            Assert.AreEqual(true, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new AndPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { false, true, true, true } }, null);

            Assert.AreEqual(false, (bool)value, "And did not work");
        }

        [TestMethod]
        public void AndPrimitive_validate()
        {
            var primitive = new AndPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "And arguments did not validate");
        }

        [TestMethod]
        public void AndPrimitive_validateTooFewArguments()
        {
            var primitive = new AndPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "And (too few) arguments did validate");
        }

        [TestMethod]
        public void AndPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new AndPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "And array arguments did validate");
        }

        [TestMethod]
        public void AndPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new AndPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "And array arguments did validate");
        }

        [TestMethod]
        public void AndPrimitive_validateArrayArguments()
        {
            var primitive = new AndPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "And array arguments did not validate");
        }
    }
}
