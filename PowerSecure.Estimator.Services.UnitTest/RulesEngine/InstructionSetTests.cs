using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
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
            repository.Insert(repository.ValidateInstructionSet("All","test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
    
            Assert.AreEqual(1, repository.Items.Count);
        }

        [TestMethod]
        public void MultipleInsertTest_oneWay()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ '*': [ 3, 'all.test' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            
            Assert.AreEqual(2, repository.Items.Count);
        }

        [TestMethod]
        public void MultipleInsertTest_theOtherWay()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test2", "{ '*': [ 3, 'all.test' ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
            
            Assert.AreEqual(2, repository.Items.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_multipleKeysInInstructionSetObject()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ], '*':[ 3 , 4] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_valueInsteadOfParameterArray()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': 3} ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { 'This is not a primitive': 3} ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullModule()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet(null, "test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullName()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", null, "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullOperation()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", null, DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullFactory()
        {
            var primitives = Primitive.Load();
            var repository = new InMemoryInstructionSetRepository();
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", DateTime.MinValue, DateTime.Now, null, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullPrimitives()
        {
            var repository = new InMemoryInstructionSetRepository();
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, null));
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void ErrorTest_invalidJson()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", "This is not json", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_missingPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ '*': [ 'y', { } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedNumberOfParametersForPrimitive()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ '+': [ 'y', { '*': [ 'x' ] } ]}", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));
        }

        [TestMethod]
        public void AllowArrayParameters_single()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = new Dictionary<string, IFunction>() { ["find"] = new TestPrimitive("find", null, p => (true, string.Empty)), ["*"] = new TestPrimitive("*", null, p => (true, string.Empty)) };
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ 'find' : [ 'z', [ 1, 'x', { '*' : [ 'y' , 3 ] }] ] }", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            Assert.AreEqual(1, repository.Items.Count);
        }

        [TestMethod]
        public void AllowArrayParameters_lotsOfNesting()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = new Dictionary<string, IFunction>() { ["find"] = new TestPrimitive("find", null, p => (true, string.Empty)), ["*"] = new TestPrimitive("*", null, p => (true, string.Empty)) };
            repository.Insert(repository.ValidateInstructionSet("All", "test", "{ 'find' : [ 'z', [ 1, 'x', { '*' : [ 'y' , ['q', [['b'],2] ]] }] ] }", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            Assert.AreEqual(1, repository.Items.Count);
        }

        [TestMethod]
        public void Evaluate_simple()
        {
            var instructionSet = new TestInstructionSet(Guid.NewGuid().ToString(), "All", "test", "{ '*': [ 2, 3 ]}", DateTime.MinValue, DateTime.Now);
            var primitives = Primitive.Load();

            var value = (decimal)instructionSet.Evaluate(null, primitives, null, null, DateTime.Now, null);

            Assert.AreEqual(6, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withParameter()
        {
            var instructionSet = new TestInstructionSet(Guid.NewGuid().ToString(), "All", "test", "{ '*': [ 'a', 3 ]}", DateTime.MinValue, DateTime.Now);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, object> { ["a"] = "2" };

            var value = (decimal)instructionSet.Evaluate(dataTable, primitives, null, null, DateTime.Now, null);

            Assert.AreEqual(6, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withNestedPrimitive()
        {
            var instructionSet = new TestInstructionSet(Guid.NewGuid().ToString(), "All", "test", "{ '*': [ 'a', { '+' : [ 'a', 3] } ]}", DateTime.MinValue, DateTime.Now);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, object> { ["a"] = "2" };

            var value = (decimal)instructionSet.Evaluate(dataTable, primitives, null, null, DateTime.Now, null);

            Assert.AreEqual(10, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withNestedPrimitiveAndMultipleParameters()
        {
            var instructionSet = new TestInstructionSet(Guid.NewGuid().ToString(), "All", "test", "{ '*': [ 'a', { '+' : [ 'b', 3] } ]}", DateTime.MinValue, DateTime.Now);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, object> { ["a"] = "2", ["b"] = "6" };

            var value = (decimal)instructionSet.Evaluate(dataTable, primitives, null, null, DateTime.Now, null);

            Assert.AreEqual(18, value, "Calculation failed");
        }

        [TestMethod]
        public void Evaluate_withMissingParameter()
        {
            var instructionSet = new TestInstructionSet(Guid.NewGuid().ToString(), "All", "test", "{ '*': [ 'a', { '+' : [ 'b', 3] } ]}", DateTime.MinValue, DateTime.Now);
            var primitives = Primitive.Load();
            var dataTable = new Dictionary<string, object> { ["a"] = "2" };

            var value = instructionSet.Evaluate(dataTable, primitives, null, null, DateTime.Now, null);
            Assert.IsNull(value, "Instruction set evaluated to a non-null value with a missing parameter.");
        }

        //[TestMethod]
        public void HappyPathTest_functionInstructionSet()
        {
            var repository = new InMemoryInstructionSetRepository();
            var primitives = Primitive.Load();
            repository.Insert(repository.ValidateInstructionSet("All", "test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", DateTime.MinValue, DateTime.Now, TestInstructionSet.Create, primitives));

            Assert.AreEqual(1, repository.Items.Count);
        }
    }
}
