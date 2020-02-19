using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class CeilingPrimitiveTests {

        [TestMethod]
        public void CeilingPrimitive_name() {
            var primitive = new CeilingPrimitive();

            Assert.AreEqual("ceiling", primitive.Name, "Ceiling name changed");
        }

        [TestMethod]
        public void CeilingPrimitive_binaryInvoke() {
            var primitive = new CeilingPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(6, (decimal)value, "Ceiling did not work");
        }

        [TestMethod]
        public void CeilingPrimitive_binaryInvoke2() {
            var primitive = new CeilingPrimitive();

            var value = primitive.Invoke(new object[] { "7", "2" }, null);

            Assert.AreEqual(8, (decimal)value, "Ceiling did not work");
        }


        [TestMethod]
        public void CeilingPrimitive_validateBinaryArguments() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Ceiling binary arguments did not validate");
        }

        [TestMethod]
        public void CeilingPrimitive_validateUnaryArguments() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Ceiling unary arguments did not validate");
        }

        [TestMethod]
        public void CeilingPrimitive_validateNoArguments() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Ceiling zero arguments did validate");
        }

        [TestMethod]
        public void CeilingPrimitive_validateSingleArrayArgument() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ '2', '5', '7'] ]"));

            Assert.IsFalse(success, "Ceiling array arguments did not validate");
        }

        [TestMethod]
        public void CeilingPrimitive_validateSingleEmptyArrayArgument() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ ] ]"));

            Assert.IsFalse(success, "Ceiling array arguments did validate");
        }

        [TestMethod]
        public void CeilingPrimitive_validateMultipleArrayArguments() {
            var primitive = new CeilingPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', ['7'] ]"));

            Assert.IsFalse(success, "Ceiling array arguments did validate");
        }
    }
}
