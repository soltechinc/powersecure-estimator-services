using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class RulesEngineTests
    {
        [TestMethod]
        public void HappyPathTest()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            
            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, string> { ["x"] = "2", ["y"] = "3", ["test"] = null }, primitives, repository, null);
            
            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test"], "Calculation is incorrect");
        }

        [TestMethod]
        public void FullDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, string> { ["x"] = "2", ["y"] = "3", ["test"] = "5" }, primitives, repository, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("5", dataSheet["test"], "Value of test was changed");
        }
    }
}
