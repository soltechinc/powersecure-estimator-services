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
    public class EqualPrimitiveTests
    {
        [TestMethod]
        public void EqualPrimitive_name()
        {
            var primitive = new EqualPrimitive();

            Assert.AreEqual("=", primitive.Name, "Equal name changed");
        }

        [TestMethod]
        public void EqualPrimitive_invokeTrue()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "2" }, null);

            Assert.AreEqual(true, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeFalse()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(false, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "6", "6" } }, null);

            Assert.AreEqual(true, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "6" } }, null);

            Assert.AreEqual(false, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeDifferingTypes_stringToDecimal()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { "2", 2m }, null);

            Assert.AreEqual(true, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeDifferingTypes_boolToDecimal()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { true, 1m }, null);

            Assert.AreEqual(true, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_invokeDifferingTypes_boolToString()
        {
            var primitive = new EqualPrimitive();

            var value = primitive.Invoke(new object[] { true, "true" }, null);

            Assert.AreEqual(true, (bool)value, "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new EqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "6", new object[] { "6" } }, null);

            Assert.AreEqual(true, (bool)value[0], "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new EqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "2", new object[] { "-2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "Equal did not work");
        }

        [TestMethod]
        public void EqualPrimitive_validate()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Equal arguments did not validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateTooFewArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Equal (too few) arguments did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateArrayArguments()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "Equal array arguments did not validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateValueWithArray()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "Equal array arguments did not validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }

        [TestMethod]
        public void EqualPrimitive_validateValueWithNestedArray()
        {
            var primitive = new EqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }
    }
}
