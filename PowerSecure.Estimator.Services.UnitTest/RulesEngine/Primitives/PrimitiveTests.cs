using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    [TestClass]
    public class PrimitiveTests
    {
        [TestMethod]
        public void Primitive_load()
        {
            var primitivesFromT4 = Primitive.Load();

            var primitivesFromAssembly = Primitive.Load(typeof(Primitive).Assembly);

            Assert.AreNotEqual(0, primitivesFromT4.Count);
            Assert.AreNotEqual(0, primitivesFromAssembly.Count);
            Assert.AreEqual(primitivesFromT4.Count, primitivesFromAssembly.Count);
        }

        [TestMethod]
        public void ConvertToDecimal()
        {
            var decimals = Primitive.ConvertToDecimal("3","16.2", "0.14", "-2");

            Assert.AreEqual(4, decimals.Length);
        }

        [TestMethod]
        public void AdditionPrimitive_binaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(8, value, "Addition did not work");
        }

        [TestMethod]
        public void AdditionPrimitive_unaryInvoke()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2" }, null);

            Assert.AreEqual(2, value, "Addition did not work");
        }

        [TestMethod]
        public void MultiplicationPrimitive()
        {
            var primitive = new MultiplicationPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(12, value, "Multiplication did not work");
        }
    }
}
