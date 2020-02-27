using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class FloorPrimitiveTests {

        [TestMethod]
        public void FloorPrimitive_name() {
            var primitive = new FloorPrimitive();

            Assert.AreEqual("floor", primitive.Name, "Floor name changed");
        }

        [TestMethod]
        public void FloorPrimitive_binaryInvoke() {
            var primitive = new FloorPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(0, (decimal)value, "Floor did not work");
        }

        [TestMethod]
        public void FloorPrimitive_binaryInvoke2() {
            var primitive = new FloorPrimitive();

            var value = primitive.Invoke(new object[] { "7", "2" }, null);

            Assert.AreEqual(6, (decimal)value, "Floor did not work");
        }


        [TestMethod]
        public void FloorPrimitive_validateBinaryArguments() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5' ]"));

            Assert.IsTrue(success, "Floor binary arguments did not validate");
        }

        [TestMethod]
        public void FloorPrimitive_validateUnaryArguments() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2' ]"));

            Assert.IsFalse(success, "Floor unary arguments did not validate");
        }

        [TestMethod]
        public void FloorPrimitive_validateNoArguments() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ ]"));

            Assert.IsFalse(success, "Floor zero arguments did validate");
        }

        [TestMethod]
        public void FloorPrimitive_validateSingleArrayArgument() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ '2', '5', '7'] ]"));

            Assert.IsFalse(success, "Floor array arguments did not validate");
        }

        [TestMethod]
        public void FloorPrimitive_validateSingleEmptyArrayArgument() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [ ] ]"));

            Assert.IsFalse(success, "Floor array arguments did validate");
        }

        [TestMethod]
        public void FloorPrimitive_validateMultipleArrayArguments() {
            var primitive = new FloorPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ '2', '5', ['7'] ]"));

            Assert.IsFalse(success, "Floor array arguments did validate");
        }
    }
}
