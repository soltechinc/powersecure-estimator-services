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
    public class NotPrimitiveTests
    {

        [TestMethod]
        public void NotPrimitive_name()
        {
            var primitive = new NotPrimitive();

            Assert.AreEqual("not", primitive.Name, "NotPrimitive name changed");
        }

        [TestMethod]
        public void NotPrimitive_binaryInvoke()
        {
            var primitive = new NotPrimitive();

            var value = (object[])primitive.Invoke(new object[] { true, false }, null);

            Assert.AreEqual(false, (bool)value[0], "NotPrimitive did not work");
            Assert.AreEqual(true, (bool)value[1], "NotPrimitive did not work");
        }

        [TestMethod]
        public void NotPrimitive_unaryInvoke()
        {
            var primitive = new NotPrimitive();

            var value = primitive.Invoke(new object[] { false }, null);

            Assert.AreEqual(true, (bool)value, "NotPrimitive did not work");
        }

        [TestMethod]
        public void NotPrimitive_unaryArrayInvoke()
        {
            var primitive = new NotPrimitive();

            var value = (object[])primitive.Invoke(new object[] { new object[] { "true", false, true } }, null);

            Assert.AreEqual(false, (bool)value[0], "NotPrimitive did not work");
            Assert.AreEqual(true, (bool)value[1], "NotPrimitive did not work");
            Assert.AreEqual(false, (bool)value[2], "NotPrimitive did not work");
        }

        [TestMethod]
        public void NotPrimitive_validateBinaryArguments()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "NotPrimitive binary arguments did not validate");
        }

        [TestMethod]
        public void NotPrimitive_validateUnaryArguments()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "NotPrimitive unary arguments did not validate");
        }

        [TestMethod]
        public void NotPrimitive_validateNoArguments()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "NotPrimitive zero arguments did validate");
        }

        [TestMethod]
        public void NotPrimitive_validateSingleArrayArgument()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ '2', '5', '7'] ]"));

            Assert.IsTrue(success, "NotPrimitive array arguments did not validate");
        }

        [TestMethod]
        public void NotPrimitive_validateSingleEmptyArrayArgument()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ ] ]"));

            Assert.IsFalse(success, "NotPrimitive array arguments did validate");
        }

        [TestMethod]
        public void NotPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new NotPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', ['7'] ]"));

            Assert.IsFalse(success, "NotPrimitive array arguments did validate");
        }
    }
}
