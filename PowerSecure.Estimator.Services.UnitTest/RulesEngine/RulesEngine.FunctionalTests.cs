﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            
            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["y"] = "3", ["all.test"] = null }, DateTime.Now, primitives, repository, null, null);
            
            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(12m, dataSheet["all.test"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_multipleInstructions()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ '*': [ 3, 'All.test' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["all.test2"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(36m, dataSheet["all.test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_shortCircuit()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ 'if': [ true, 'All.test', 'All.test3' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test3", "{ '*': [ 'z', 'z' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["z"] = "nan", ["all.test2"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(4, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(12m, dataSheet["all.test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_shortCircuitMissingParameter()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ 'if': [ true, 'All.test', 'All.test3' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test3", "{ '*': [ 'z', 'z' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["all.test2"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(12m, dataSheet["all.test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void HappyPathTest_datedInstructionSets()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now.AddDays(-2), TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 3 ] } ]} ", DateTime.MinValue.AddDays(1), DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["y"] = "3", ["all.test"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(15m, dataSheet["all.test"], "Calculation is incorrect");
        }

        [TestMethod]
        public void ErrorTest_parameterTypingError()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ 'if': [ true, 'All.test', 'All.test3' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test3", "{ '*': [ 'z', 'z' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = 2, ["y"] = "3", ["z"] = "nan", ["all.test3"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(4, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.IsNull(dataSheet["all.test3"], "Calculation is incorrect");
        }

        [TestMethod]
        public void ErrorTest_missingParameter()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ '*': [ 3, 'All.test' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["all.test2"] = null }, DateTime.Now, primitives, repository, null, null);
            Assert.AreEqual(2, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.IsNull(dataSheet["all.test2"], "Calculation returned a non-null value with a missing parameter");
        }

        [TestMethod]
        public void PartialDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ '*': [ 3, 'All.test' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["all.test"] = 4, ["all.test2"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(2, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(12m, dataSheet["all.test2"], "Calculation is incorrect");
        }

        [TestMethod]
        public void FullDataSheet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["x"] = "2", ["y"] = "3", ["all.test"] = "5" }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(3, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual("5", dataSheet["all.test"], "Value of test was changed");
        }

        [TestMethod]
        public void HappyPathTest_isEmpty()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { 'isempty': [ 'y' ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            var engine = new Components.RulesEngine.RulesEngine();

            var dataSheet = engine.EvaluateDataSheet(new Dictionary<string, object> { ["all.test"] = null }, DateTime.Now, primitives, repository, null, null);

            Assert.AreEqual(1, dataSheet.Count, "Count of items in data sheet is incorrect");
            Assert.AreEqual(true, dataSheet["all.test"], "Calculation is incorrect");
        }
    }
}
