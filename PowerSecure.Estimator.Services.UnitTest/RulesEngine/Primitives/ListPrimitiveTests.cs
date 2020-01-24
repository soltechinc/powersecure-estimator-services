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
    public class ListPrimitiveTests
    {
        [TestMethod]
        public void ListPrimitive_name()
        {
            var primitive = new ListPrimitive();

            Assert.AreEqual("list", primitive.Name, "List name changed");
        }

        [TestMethod]
        public void ListPrimitive_invoke()
        {
            var primitive = new ListPrimitive();

            var value = (object[])primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual("2", value[0], "List did not work");
            Assert.AreEqual("6", value[1], "List did not work");
        }

        [TestMethod]
        public void ListPrimitive_validateBinaryArguments()
        {
            var primitive = new ListPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "List binary arguments did not validate");
        }

        [TestMethod]
        public void ListPrimitive_validateUnaryArguments()
        {
            var primitive = new ListPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsTrue(success, "List unary arguments did not validate");
        }

        [TestMethod]
        public void ListPrimitive_validateNoArguments()
        {
            var primitive = new ListPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "List zero arguments did validate");
        }

        [TestMethod]
        public void ListPrimitive_validateNestedArrayArgument()
        {
            var primitive = new ListPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ ] ]"));

            Assert.IsFalse(success, "List array arguments did validate");
        }
    }
}
