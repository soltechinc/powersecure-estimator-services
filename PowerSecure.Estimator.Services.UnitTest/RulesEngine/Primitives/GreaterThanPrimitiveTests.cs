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
    public class GreaterThanPrimitiveTests
    {

        [TestMethod]
        public void GreaterThanPrimitive_name()
        {
            var primitive = new GreaterThanPrimitive();

            Assert.AreEqual(">", primitive.Name, "GreaterThan name changed");
        }

        [TestMethod]
        public void GreaterThanPrimitive_invokeTrue()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2" }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_invokeFalse()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "6" } }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "2" } }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "6", "7", "2" } }, null);

            Assert.AreEqual(true, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new GreaterThanPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "8", "6", "7", "10" } }, null);

            Assert.AreEqual(false, (bool)value, "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new GreaterThanPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "8", new object[] { "6" } }, null);

            Assert.AreEqual(true, (bool)value[0], "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new GreaterThanPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "2", new object[] { "2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "GreaterThan did not work");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validate()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "GreaterThan arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateTooFewArguments()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "GreaterThan (too few) arguments did validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "GreaterThan array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "GreaterThan array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateArrayArguments()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "GreaterThan array arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateValueWithArray()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "GreaterThan array arguments did not validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "GreaterThan array arguments did validate");
        }

        [TestMethod]
        public void GreaterThanPrimitive_validateValueWithNestedArray()
        {
            var primitive = new GreaterThanPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "GreaterThan array arguments did validate");
        }
    }
}
