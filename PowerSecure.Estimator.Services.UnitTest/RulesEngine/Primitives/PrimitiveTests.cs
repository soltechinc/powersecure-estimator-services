using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives
{
    [TestClass]
    public class PrimitiveTests
    {
        [TestMethod]
        public void Load_bothMethodsMatch()
        {
            var primitivesFromT4 = Primitive.Load();

            var primitivesFromAssembly = Primitive.Load(typeof(Primitive).Assembly);

            Assert.AreNotEqual(0, primitivesFromT4.Count, "No primitives were loaded via T4-generated class");
            Assert.AreNotEqual(0, primitivesFromAssembly.Count, "No primitives were loaded via reflection");
            Assert.AreEqual(primitivesFromT4.Count, primitivesFromAssembly.Count, "Differing numbers of primitives loaded via reflection and T4-generated class");
        }
    }
}
