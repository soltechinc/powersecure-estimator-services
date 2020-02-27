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
    public class IsEmptyPrimitiveTests
    {

        [TestMethod]
        public void IsNullPrimitive_name()
        {
            var primitive = new IsEmptyPrimitive();

            Assert.AreEqual("isempty", primitive.Name, "IsNull name changed");
        }

        [TestMethod]
        public void IsNullPrimitive_invokeValue()
        {
            var primitive = new IsEmptyPrimitive();

            var value = (bool)primitive.Invoke(new object[] { "-3" }, null);

            Assert.AreEqual(false, value, "IsNull did not work");
        }

        [TestMethod]
        public void IsNullPrimitive_invokeIsNull()
        {
            var primitive = new IsEmptyPrimitive();

            var value = (bool)primitive.Invoke(new object[] { null }, null);

            Assert.AreEqual(true, value, "IsNull did not work");
        }

        [TestMethod]
        public void IsNullPrimitive_validateTooFewArguments()
        {
            var primitive = new IsEmptyPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[  ]"));

            Assert.IsFalse(success, "IsNull (too few) arguments did validate");
        }

        [TestMethod]
        public void IsNullPrimitive_validateTooManyArguments()
        {
            var primitive = new IsEmptyPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "IsNull (too many) arguments did validate");
        }

        [TestMethod]
        public void IsNullPrimitive_validateArrayArguments()
        {
            var primitive = new IsEmptyPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "IsNull array arguments did validate");
        }

        [TestMethod]
        public void IsNullPrimitive_validate()
        {
            var primitive = new IsEmptyPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '3' ]"));

            Assert.IsTrue(success, "IsNull arguments did not validate");
        }
    }
}
