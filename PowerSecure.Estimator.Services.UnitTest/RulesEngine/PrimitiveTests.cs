using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class PrimitiveTests
    {
        [TestMethod]
        public void PrimitiveLoad_fromAssembly()
        {
            var primitives = Primitive.Load();

            Assert.AreNotEqual(0, primitives.Count);
        }

        [TestMethod]
        public void AdditionPrimitive()
        {
            var primitive = new AdditionPrimitive();

            var value = primitive.Invoke(new object[] { "2", "6" }, null);

            Assert.AreEqual(8, value, "Addition did not work");
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
