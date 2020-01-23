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
    public class LessThanPrimitiveTests
    {

        [TestMethod]
        public void LessThanPrimitive_name()
        {
            var primitive = new LessThanPrimitive();

            Assert.AreEqual("<", primitive.Name, "LessThan name changed");
        }

        [TestMethod]
        public void LessThanPrimitive_invokeTrue()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { "-6", "2" }, null);

            Assert.AreEqual(true, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_invokeFalse()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { "2", "-6" }, null);

            Assert.AreEqual(false, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "6" } }, null);

            Assert.AreEqual(true, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "2" } }, null);

            Assert.AreEqual(false, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "6", "7", "2" } }, null);

            Assert.AreEqual(true, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new LessThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "-8", "6", "7", "-10" } }, null);

            Assert.AreEqual(false, (bool)value, "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new LessThanPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "-8", new object[] { "6" } }, null);

            Assert.AreEqual(true, (bool)value[0], "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new LessThanPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "2", new object[] { "2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "LessThan did not work");
        }

        [TestMethod]
        public void LessThanPrimitive_validate()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "LessThan arguments did not validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateTooFewArguments()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "LessThan (too few) arguments did validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "LessThan array arguments did validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "LessThan array arguments did validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateArrayArguments()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "LessThan array arguments did not validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateValueWithArray()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "LessThan array arguments did not validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "LessThan array arguments did validate");
        }

        [TestMethod]
        public void LessThanPrimitive_validateValueWithNestedArray()
        {
            var primitive = new LessThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "LessThan array arguments did validate");
        }
    }
}
