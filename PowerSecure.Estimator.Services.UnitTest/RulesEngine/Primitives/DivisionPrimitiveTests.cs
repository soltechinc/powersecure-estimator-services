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
    public class DivisionPrimitiveTests
    {

        [TestMethod]
        public void DivisionPrimitive_name()
        {
            var primitive = new DivisionPrimitive();

            Assert.AreEqual("/", primitive.Name, "Division name changed");
        }

        [TestMethod]
        public void DivisionPrimitive_binaryInvoke()
        {
            var primitive = new DivisionPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2" }, null);

            Assert.AreEqual(3, (decimal)value, "Division did not work");
        }

        [TestMethod]
        public void DivisionPrimitive_ternaryInvoke()
        {
            var primitive = new DivisionPrimitive();

            var value = primitive.Invoke(new object[] { "30", "3", "2" }, null);

            Assert.AreEqual(5, (decimal)value, "Division did not work");
        }

        [TestMethod]
        public void DivisionPrimitive_validateTooFewArguments()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Division arguments (too few) did validate");
        }

        [TestMethod]
        public void DivisionPrimitive_binaryValidate()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '6', '3' ]"));

            Assert.IsTrue(success, "Division arguments did not validate");
        }

        [TestMethod]
        public void DivisionPrimitive_ternaryValidate()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '6', '3', '2' ]"));

            Assert.IsTrue(success, "Division arguments did not validate");
        }

        [TestMethod]
        public void DivisionPrimitive_validateArrayArguments()
        {
            var primitive = new DivisionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2' ]"));

            Assert.IsFalse(success, "Division array arguments did validate");
        }
    }
}
