using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using Newtonsoft.Json;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class InstructionSetTests
    {
        [TestMethod]
        public void HappyPathTest()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
    
            Assert.AreEqual(1, repository.Items.Count);

            var instructionSet = repository.Items.Values.First();
            
            Assert.AreEqual(2, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(0, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
            Assert.AreEqual(0, instructionSet.Sequence, "Sequence number does not match");
        }

        [TestMethod]
        public void MultipleInsertTest_oneWay()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);
            
            Assert.AreEqual(2, repository.Items.Count);

            var instructionSet = repository.Items["test2"];

            Assert.AreEqual(0, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(1, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
            Assert.AreEqual(1, instructionSet.Sequence, "Sequence number does not match");
        }

        [TestMethod]
        public void MultipleInsertTest_theOtherWay()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            
            Assert.AreEqual(2, repository.Items.Count);

            var instructionSet = repository.Items["test2"];

            Assert.AreEqual(0, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(1, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
            Assert.AreEqual(1, instructionSet.Sequence, "Sequence number does not match");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_multipleKeysInInstructionSetObject()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ], '*':[ 3 , 4] } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_valueInsteadOfParameterArray()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': 3} ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { 'This is not a primitive': 3} ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullName()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew(null, "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullOperation()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", null, repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullRepository()
        {
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", null, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullPrimitives()
        {
            var repository = new InMemoryInstructionSetRepository();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", repository, null);
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void ErrorTest_invalidJson()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", "This is not json", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_missingPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedNumberOfParametersForPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            InstructionSet.InsertNew("test", "{ '+': [ 'y', { '*': [ 'x' ] } ]}", repository, primitives);
        }

        [TestMethod]
        public void AllowArrayParameters_single()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = new Dictionary<string, IPrimitive>() { ["find"] = new TestPrimitive("find", true, null, p => Tuple.Create(true, string.Empty)), ["*"] = new TestPrimitive("*", true, null, p => Tuple.Create(true, string.Empty)) };
            InstructionSet.InsertNew("test", "{ 'find' : [ 'z', [ 1, 'x', { '*' : [ 'y' , 3 ] }] ] }", repository, primitives);

            Assert.AreEqual(1, repository.Items.Count);

            var instructionSet = repository.Items.Values.First();

            Assert.AreEqual(3, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(0, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }

        [TestMethod]
        public void AllowArrayParameters_lotsOfNesting()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = new Dictionary<string, IPrimitive>() { ["find"] = new TestPrimitive("find", true, null, p => Tuple.Create(true, string.Empty)), ["*"] = new TestPrimitive("*", true, null, p => Tuple.Create(true, string.Empty)) };
            InstructionSet.InsertNew("test", "{ 'find' : [ 'z', [ 1, 'x', { '*' : [ 'y' , ['q', [['b'],2] ]] }] ] }", repository, primitives);

            Assert.AreEqual(1, repository.Items.Count);

            var instructionSet = repository.Items.Values.First();

            Assert.AreEqual(5, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(0, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }

        [TestMethod]
        public void Evaluate_simple()
        {
            var instructionSet = new InstructionSet("test", "{ '*': [ 2, 3 ]}", new string[] { }, new string[] { }, 0);
            var primitives = Primitive.Load();

            decimal value = instructionSet.Evaluate(null, primitives, null);

            Assert.AreEqual(6, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withParameter()
        {
            var instructionSet = new InstructionSet("test", "{ '*': [ 'a', 3 ]}", new string[] { "a" }, new string[] { }, 0);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, string> { ["a"] = "2" };

            decimal value = instructionSet.Evaluate(dataTable, primitives, null);

            Assert.AreEqual(6, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withNestedPrimitive()
        {
            var instructionSet = new InstructionSet("test", "{ '*': [ 'a', { '+' : [ 'a', 3] } ]}", new string[] { "a" }, new string[] { }, 0);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, string> { ["a"] = "2" };

            decimal value = instructionSet.Evaluate(dataTable, primitives, null);

            Assert.AreEqual(10, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withNestedPrimitiveAndMultipleParameters()
        {
            var instructionSet = new InstructionSet("test", "{ '*': [ 'a', { '+' : [ 'b', 3] } ]}", new string[] { "a", "b" }, new string[] { }, 0);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, string> { ["a"] = "2", ["b"] = "6" };

            decimal value = instructionSet.Evaluate(dataTable, primitives, null);

            Assert.AreEqual(18, value, "Calculation failed");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Evaluate_withMissingParameter()
        {
            var instructionSet = new InstructionSet("test", "{ '*': [ 'a', { '+' : [ 'b', 3] } ]}", new string[] { "a" }, new string[] { }, 0);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, string> { ["a"] = "2" };

            decimal value = instructionSet.Evaluate(dataTable, primitives, null);
        }

        [TestMethod]
        public void FindTest()
        {
            var instructionSet = new InstructionSet("test", "{ 'find': [ 'dataSetName', [['search','value'], ['search2','value2']], 'returnValueAttribute' ]}", new string[] { }, new string[] { }, 0);
            var primitives = Primitive.Load();

            decimal value = instructionSet.Evaluate(null, primitives, new InMemoryReferenceDataRepository());

            Assert.AreEqual(0, value, "Calculation failed");
        }
    }
}
