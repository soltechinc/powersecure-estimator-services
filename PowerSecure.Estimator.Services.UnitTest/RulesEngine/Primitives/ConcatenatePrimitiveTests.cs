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
    public class ConcatenatePrimitiveTests
    {

        [TestMethod]
        public void ConcatenatePrimitive_name()
        {
            var primitive = new ConcatenatePrimitive();

            Assert.AreEqual("buildstring", primitive.Name, "Concatenate name changed");
        }

        [TestMethod]
        public void ConcatenatePrimitive_binaryInvoke()
        {
            var primitive = new ConcatenatePrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual("$26", value.ToString(), "Concatenate did not work");
        }

        [TestMethod]
        public void ConcatenatePrimitive_unaryInvoke()
        {
            var primitive = new ConcatenatePrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual("$2", value.ToString(), "Concatenate did not work");
        }

        [TestMethod]
        public void ConcatenatePrimitive_unaryArrayInvoke()
        {
            var primitive = new ConcatenatePrimitive();

            var value = primitive.Invoke(new object[] { new object[] { "2", 6, "Hello", "World" } }, null);

            Assert.AreEqual("$26HelloWorld", value.ToString(), "Concatenate did not work");
        }

        [TestMethod]
        public void ConcatenatePrimitive_validateBinaryArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Concatenate binary arguments did not validate");
        }

        [TestMethod]
        public void ConcatenatePrimitive_validateUnaryArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "Concatenate unary arguments did not validate");
        }

        [TestMethod]
        public void ConcatenatePrimitive_validateNoArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Concatenate zero arguments did validate");
        }
        
        [TestMethod]
        public void ConcatenatePrimitive_validateArrayArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ '4' ] ]"));

            Assert.IsTrue(success, "Concatenate array arguments did not validate");
        }

        [TestMethod]
        public void ConcatenatePrimitive_validateMultipleArrayArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '4', [] ]"));

            Assert.IsFalse(success, "Concatenate array arguments did validate");
        }

        [TestMethod]
        public void ConcatenatePrimitive_validateEmptyArrayArguments()
        {
            var primitive = new ConcatenatePrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [] ]"));

            Assert.IsFalse(success, "Concatenate array arguments did validate");
        }
    }
}
