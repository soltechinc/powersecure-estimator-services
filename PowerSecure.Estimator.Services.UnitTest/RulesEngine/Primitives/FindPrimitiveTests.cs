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
    public class FindPrimitiveTests
    {

        [TestMethod]
        public void FindPrimitive_name()
        {
            var primitive = new FindPrimitive();

            Assert.AreEqual("find", primitive.Name, "Find name changed");
        }

        [TestMethod]
        public void FindPrimitive_validateTooFewArguments()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments (too few) did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateTooManyArguments()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments (too many) did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate1stArgumentArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate3rdArgumentArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', [] ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateNoArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '2', '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateEmptyArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateOddArray()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ '3' ], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateCriteriaArrayWithEmptyPair()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ ] ], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validateCriteriaArrayWithTriad()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ [ '2', '2', '2' ] ], '2' ]"));

            Assert.IsFalse(success, "Find arguments did validate");
        }

        [TestMethod]
        public void FindPrimitive_validate()
        {
            var primitive = new FindPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', [ ['3', '4'],['5','6'] ],'2', '2' ]"));

            Assert.IsTrue(success, "Find arguments did not validate");
        }
    }
}
