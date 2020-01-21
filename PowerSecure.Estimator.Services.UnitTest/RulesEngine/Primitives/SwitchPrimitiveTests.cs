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
    public class SwitchPrimitiveTests
    {

        [TestMethod]
        public void SwitchPrimitive_name()
        {
            var primitive = new SwitchPrimitive();

            Assert.AreEqual("switch", primitive.Name, "Switch name changed");
        }

        [TestMethod]
        public void SwitchPrimitive_validateTooFewArguments()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "Switch arguments (too few) did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateTooManyArguments()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Switch arguments (too many) did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validate1stArgumentArray()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validate3rdArgumentArray()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', [] ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateNoArray()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateEmptyArray()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [], '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateOddArray()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '3' ], '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateCriteriaArrayWithEmptyPair()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ ] ], '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validateCriteriaArrayWithTriad()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2', '2', '2' ] ], '2' ]"));

            Assert.IsFalse(success, "Switch arguments did validate");
        }

        [TestMethod]
        public void SwitchPrimitive_validate()
        {
            var primitive = new SwitchPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ['3', '4'],['5','6'] ], '2' ]"));

            Assert.IsTrue(success, "Switch arguments did not validate");
        }

        [TestMethod]
        public void SwitchPrimitive_invokeTrueCase()
        {
            var primitive = new SwitchPrimitive();

            var value = primitive.Invoke(new object[] { true, new object[] { new object[] { false, "falseValue" }, new object[] { true, "trueValue" } }, "defaultValue" }, null);

            Assert.AreEqual(value, "trueValue", "True value did not return");
        }

        [TestMethod]
        public void SwitchPrimitive_invokeFalseCase()
        {
            var primitive = new SwitchPrimitive();

            var value = primitive.Invoke(new object[] { false, new object[] { new object[] { false, "falseValue" }, new object[] { true, "trueValue" } }, "defaultValue" }, null);

            Assert.AreEqual(value, "falseValue", "False value did not return");
        }

        [TestMethod]
        public void SwitchPrimitive_invokeDefaultCase()
        {
            var primitive = new SwitchPrimitive();

            var value = primitive.Invoke(new object[] { 1, new object[] { new object[] { 2, "falseValue" }, new object[] { 3, "trueValue" } }, "defaultValue" }, null);

            Assert.AreEqual(value, "defaultValue", "Default value did not return");
        }

        [TestMethod]
        public void SwitchPrimitive_invokeUsingStrings()
        {
            var primitive = new SwitchPrimitive();

            var value = primitive.Invoke(new object[] { "a", new object[] { new object[] {"a", "stringValue" }, new object[] { 3, "trueValue" } }, "defaultValue" }, null);

            Assert.AreEqual(value, "stringValue", "String value did not return");
        }
    }
}
