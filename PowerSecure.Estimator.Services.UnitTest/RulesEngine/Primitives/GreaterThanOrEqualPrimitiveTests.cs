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
    public class GreaterThanOrEqualPrimitiveTests
    {

        [TestMethod]
        public void GreaterThanPrimitive_name()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            Assert.AreEqual(">=", primitive.Name, "GreaterThanOrEqual name changed");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_invokeTrue()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2" }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_invokeFalse()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "8" } }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "1", "2" } }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "6", "7", "2" } }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "6", "7", "10" } }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "8", new object[] { "8" } }, null);

            Assert.AreEqual(true, (bool)value[0], "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "1", new object[] { "2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "GreaterThanOrEqual did not work");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validate()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "GreaterThanOrEqual arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateTooFewArguments()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "GreaterThanOrEqual (too few) arguments did validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "GreaterThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "GreaterThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateArrayArguments()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "GreaterThanOrEqual array arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateValueWithArray()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "GreaterThanOrEqual array arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "GreaterThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanOrEqualPrimitive_validateValueWithNestedArray()
        {
            var primitive = new GreaterThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "GreaterThanOrEqual array arguments did validate");
        }
    }
}