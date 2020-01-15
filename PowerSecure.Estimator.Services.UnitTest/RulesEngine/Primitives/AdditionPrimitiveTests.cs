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
    public class AdditionPrimitiveTests
    {

        [TestMethod]
        public void AdditionPrimitive_name()
        {
            var primitive = new AdditionPrimitive();

            Assert.AreEqual("+", primitive.Name, "Addition name changed");
        }

        [TestMethod]
        public void AdditionPrimitive_binaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(8, (decimal)value, "Addition did not work");
        }

        [TestMethod]
        public void AdditionPrimitive_unaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(2, (decimal)value, "Addition did not work");
        }

        [TestMethod]
        public void AdditionPrimitive_validateBinaryArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Addition binary arguments did not validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateUnaryArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Addition unary arguments did not validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateNoArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Addition zero arguments did validate");
        }

        [TestMethod]
        public void AdditionPrimitive_validateArrayArguments()
        {
            var primitive = new AdditionPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Addition array arguments did validate");
        }
    }
}
