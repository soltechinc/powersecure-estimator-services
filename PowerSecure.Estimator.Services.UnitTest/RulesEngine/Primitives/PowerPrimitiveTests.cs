using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class PowerPrimitiveTests {

        [TestMethod]
        public void PowerPrimitive_name() {
            var primitive = new PowerPrimitive();

            Assert.AreEqual("^", primitive.Name, "Power name changed");
        }

        [TestMethod]
        public void PowerPrimitive_binaryInvoke() {
            var primitive = new PowerPrimitive();

            var value = primitive.Invoke(new object[] { "6", "2" }, null);

            Assert.AreEqual(36, (decimal)value, "Power did not work");
        }

        [TestMethod]
        public void PowerPrimitive_binaryInvoke2() {
            var primitive = new PowerPrimitive();

            var value = primitive.Invoke(new object[] { "7", "2" }, null);

            Assert.AreEqual(49, (decimal)value, "Power did not work");
        }


        [TestMethod]
        public void PowerPrimitive_validateBinaryArguments() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Power binary arguments did not validate");
        }

        [TestMethod]
        public void PowerPrimitive_validateUnaryArguments() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Power unary arguments did not validate");
        }

        [TestMethod]
        public void PowerPrimitive_validateNoArguments() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Power zero arguments did validate");
        }

        [TestMethod]
        public void PowerPrimitive_validateSingleArrayArgument() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ '2', '5', '7'] ]"));

            Assert.IsFalse(success, "Power array arguments did not validate");
        }

        [TestMethod]
        public void PowerPrimitive_validateSingleEmptyArrayArgument() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ ] ]"));

            Assert.IsFalse(success, "Power array arguments did validate");
        }

        [TestMethod]
        public void PowerPrimitive_validateMultipleArrayArguments() {
            var primitive = new PowerPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', ['7'] ]"));

            Assert.IsFalse(success, "Power array arguments did validate");
        }
    }
}
