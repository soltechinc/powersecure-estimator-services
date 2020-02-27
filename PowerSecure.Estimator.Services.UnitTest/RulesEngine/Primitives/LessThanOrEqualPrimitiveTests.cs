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
    public class LessThanOrEqualPrimitiveTests
    {

        [TestMethod]
        public void LessThanOrEqualPrimitive_name()
        {
            var primitive = new LessThanOrEqualPrimitive();

            Assert.AreEqual("<=", primitive.Name, "LessThanOrEqual name changed");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_invokeTrue()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { "-6", "2" }, null);

            Assert.AreEqual(true, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_invokeFalse()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "-6" }, null);

            Assert.AreEqual(false, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "-8" } }, null);

            Assert.AreEqual(true, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "-2" } }, null);

            Assert.AreEqual(false, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "6", "7", "2" } }, null);

            Assert.AreEqual(true, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "6", "7", "-10" } }, null);

            Assert.AreEqual(false, (bool)value, "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "6", new object[] { "6" } }, null);

            Assert.AreEqual(true, (bool)value[0], "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new LessThanOrEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "2", new object[] { "-2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "LessThanOrEqual did not work");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validate()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "LessThanOrEqual arguments did not validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateTooFewArguments()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "LessThanOrEqual (too few) arguments did validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "LessThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "LessThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateArrayArguments()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "LessThanOrEqual array arguments did not validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateValueWithArray()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "LessThanOrEqual array arguments did not validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "LessThanOrEqual array arguments did validate");
        }

        [TestMethod]
        public void LessThanOrEqualPrimitive_validateValueWithNestedArray()
        {
            var primitive = new LessThanOrEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "LessThanOrEqual array arguments did validate");
        }
    }
}
