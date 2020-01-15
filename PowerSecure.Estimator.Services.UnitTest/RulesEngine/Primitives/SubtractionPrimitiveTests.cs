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
    public class SubtractionPrimitiveTests
    {

        [TestMethod]
        public void SubtractionPrimitive_name()
        {
            var primitive = new SubtractionPrimitive();

            Assert.AreEqual("-", primitive.Name, "Subtraction name changed");
        }

        [TestMethod]
        public void SubtractionPrimitive_binaryInvoke()
        {
            var primitive = new SubtractionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(-4, (decimal)value, "Subtraction did not work");
        }

        [TestMethod]
        public void SubtractionPrimitive_unaryInvoke()
        {
            var primitive = new SubtractionPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(-2, (decimal)value, "Subtraction did not work");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateBinaryArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Subtraction binary arguments did not validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateUnaryArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Subtraction unary arguments did not validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateNoArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Subtraction zero arguments did validate");
        }

        [TestMethod]
        public void SubtractionPrimitive_validateArrayArguments()
        {
            var primitive = new SubtractionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Subtraction array arguments did validate");
        }
    }
}
