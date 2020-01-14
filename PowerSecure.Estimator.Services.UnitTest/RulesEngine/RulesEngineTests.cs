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
        public void HappyPathTest_singleInstruction()
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
        public void HappyPathTest_multipleInstructions()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, string> { ["x"] = "2", ["y"] = "3", ["test2"] = null }, primitives, repository, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("36", dataSheet["test2"], "Calculation is incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_missingParameter()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, string> { ["x"] = "2", ["test2"] = null }, primitives, repository, null);
        }

        [TestMethod]
        public void PartialDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, string> { ["test"] = "4", ["test2"] = null }, primitives, repository, null);

            Assert.AreEqual(2, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test2"], "Calculation is incorrect");
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
