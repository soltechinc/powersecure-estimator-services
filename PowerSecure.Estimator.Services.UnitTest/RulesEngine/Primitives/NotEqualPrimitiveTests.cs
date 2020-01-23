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
    public class NotEqualPrimitiveTests
    {

        [TestMethod]
        public void NotEqualPrimitive_name()
        {
            var primitive = new NotEqualPrimitive();

            Assert.AreEqual("!=", primitive.Name, "NotEqual name changed");
        }

        [TestMethod]
        public void NotEqualPrimitive_invokeTrue()
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { "1", "2" }, null);

            Assert.AreEqual(true, (bool)value, "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_invokeFalse()
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { "6", "6" }, null);

            Assert.AreEqual(false, (bool)value, "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "3", "6" } }, null);

            Assert.AreEqual(true, (bool)value, "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new NotEqualPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", "2" } }, null);

            Assert.AreEqual(false, (bool)value, "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_valueWithArrayInvokeTrue()
        {
            var primitive = new NotEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "6", new object[] { "-6" } }, null);

            Assert.AreEqual(true, (bool)value[0], "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_valueWithArrayInvokeFalse()
        {
            var primitive = new NotEqualPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "-2", new object[] { "-2" } }, null);

            Assert.AreEqual(false, (bool)value[0], "NotEqual did not work");
        }

        [TestMethod]
        public void NotEqualPrimitive_validate()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "NotEqual arguments did not validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateTooFewArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "NotEqual (too few) arguments did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "NotEqual array arguments did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "NotEqual array arguments did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateArrayArguments()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "NotEqual array arguments did not validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateValueWithArray()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '2' ]]"));

            Assert.IsTrue(success, "NotEqual array arguments did not validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateValueWithEmptyArray()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ]]"));

            Assert.IsFalse(success, "NotEqual array arguments did validate");
        }

        [TestMethod]
        public void NotEqualPrimitive_validateValueWithNestedArray()
        {
            var primitive = new NotEqualPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2' ] ]]"));

            Assert.IsFalse(success, "NotEqual array arguments did validate");
        }
    }
}
