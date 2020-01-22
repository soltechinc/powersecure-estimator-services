using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives {
    [TestClass]
    public class OrPrimitiveTests {

        [TestMethod]
        public void TestPrimitive_name() {
            var primitive = new OrPrimitive();
            Assert.AreEqual("or", primitive.Name, "Equal name changed");
        }

        [TestMethod]
        public void TestPrimitive_Validate() {
            var primitive = new OrPrimitive();
            (var success, var message) = primitive.Validate(JToken.Parse("[4,4,6]"));
            Assert.IsTrue(success, message);
        }

        [TestMethod]
        public void TestPrimitive_invokeWithTrue() {
            var primitive = new OrPrimitive();
            var value = primitive.Invoke(new object[] { 2, 1 }, null);
            Assert.AreEqual(value, true);
        }


        [TestMethod]
        public void TestPrimitive_invokeWithTrueWithMoreValues() {
            var primitive = new OrPrimitive();
            var value = primitive.Invoke(new object[] { 0, 3, 4, -1, 5, 0 }, null);
            Assert.AreEqual(value, true);
        }


        [TestMethod]
        public void TestPrimitive_Validate_FewArguments() {
            var primitive = new OrPrimitive();
            (var success, var message) = primitive.Validate(JToken.Parse("[4]"));
            Assert.IsTrue(success, message);
        }

        [TestMethod]
        public void TestPrimitive_validateArrayArguments() {
            var primitive = new OrPrimitive();

            (var success, var message) = primitive.Validate(JToken.Parse("[ [], '2', '2', '2' ]"));

            Assert.IsFalse(success, "Equal array arguments did validate");
        }
    }
}
