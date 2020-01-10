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
            var primitives = Primitive.LoadFromAssembly();

            Assert.AreNotEqual(0, primitives.Count);
        }
    }
}
