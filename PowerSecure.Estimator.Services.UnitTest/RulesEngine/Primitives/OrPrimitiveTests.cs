using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class OrPrimitiveTests
    {
        [TestMethod]
        public void OrPrimitive_name()
        {
            var primitive = new OrPrimitive();

            Assert.AreEqual("or", primitive.Name, "Or name changed");
        }

        [TestMethod]
        public void OrPrimitive_invokeTrue()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { true, false }, null);

            Assert.AreEqual(true, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_invokeFalse()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { false, false }, null);

            Assert.AreEqual(false, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_unaryArrayInvokeTrue()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { false, true } }, null);

            Assert.AreEqual(true, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_unaryArrayInvokeFalse()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { false, false } }, null);

            Assert.AreEqual(false, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_unaryBiggerArrayInvokeTrue()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { false, false, true, false } }, null);

            Assert.AreEqual(true, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_unaryBiggerArrayInvokeFalse()
        {
            var primitive = new OrPrimitive();

            var value = primitive.Invoke(new object[] { new object[] { false, false, false, false } }, null);

            Assert.AreEqual(false, (bool)value, "Or did not work");
        }

        [TestMethod]
        public void OrPrimitive_validate()
        {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Or arguments did not validate");
        }

        [TestMethod]
        public void OrPrimitive_validateTooFewArguments()
        {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Or (too few) arguments did validate");
        }

        [TestMethod]
        public void OrPrimitive_validateMultipleArrayArguments()
        {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Or array arguments did validate");
        }

        [TestMethod]
        public void OrPrimitive_validateEmptyArrayArgument()
        {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Or array arguments did validate");
        }

        [TestMethod]
        public void OrPrimitive_validateArrayArguments()
        {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ['5', '2'] ]"));

            Assert.IsTrue(success, "Or array arguments did not validate");
        }
    }
}
