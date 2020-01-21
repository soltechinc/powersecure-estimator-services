using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class RulesEngineFunctionalTests
    {
        [TestMethod]
        public void HappyPathTest_singleInstruction()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            
            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["y"] = "3", ["test"] = null }, primitives, repository, null);
            
            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_multipleInstructions()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            repository.InsertNew("test2", "{ '*': [ 3, 'test' ]}", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["test2"] = null }, primitives, repository, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("36", dataSheet["test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_shortCircuit()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            repository.InsertNew("test2", "{ 'if': [ true, 'test', 'test3' ]}", InstructionSet.Create, primitives);
            repository.InsertNew("test3", "{ '*': [ 'z', 'z' ]}", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["z"] = "nan", ["test2"] = null }, primitives, repository, null);

            Assert.AreEqual(4, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test2"], "Calculation is incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ErrorTest_parameterTypingError()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            repository.InsertNew("test2", "{ 'if': [ true, 'test', 'test3' ]}", InstructionSet.Create, primitives);
            repository.InsertNew("test3", "{ '*': [ 'z', 'z' ]}", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["z"] = "nan", ["test3"] = null }, primitives, repository, null);

            Assert.AreEqual(4, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test2"], "Calculation is incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_missingParameter()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            repository.InsertNew("test2", "{ '*': [ 3, 'test' ]}", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["test2"] = null }, primitives, repository, null);
        }

        [TestMethod]
        public void PartialDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);
            repository.InsertNew("test2", "{ '*': [ 3, 'test' ]}", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["test"] = 4, ["test2"] = null }, primitives, repository, null);

            Assert.AreEqual(2, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("12", dataSheet["test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void FullDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", InstructionSet.Create, primitives);

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["y"] = "3", ["test"] = "5" }, primitives, repository, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("5", dataSheet["test"], "Value of test was changed");
        }
    }
}
