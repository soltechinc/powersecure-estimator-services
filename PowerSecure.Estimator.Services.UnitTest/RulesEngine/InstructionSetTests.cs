using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using Newtonsoft.Json;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Primitives;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class InstructionSetTests
    {
        [TestMethod]
        public void HappyPathTest()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
    
            Assert.AreEqual(1, repository.Items.Count);

            InstructionSet instructionSet = repository.Items.Values.First();
            
            Assert.AreEqual(2, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(0, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }

        [TestMethod]
        public void MultipleInsertTest_oneWay()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);
            
            Assert.AreEqual(2, repository.Items.Count);

            InstructionSet instructionSet = repository.Items["test2"];

            Assert.AreEqual(0, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(1, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }

        [TestMethod]
        public void MultipleInsertTest_theOtherWay()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test2", "{ '*': [ 3, 'test' ]}", repository, primitives);
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
            
            Assert.AreEqual(2, repository.Items.Count);

            InstructionSet instructionSet = repository.Items["test2"];

            Assert.AreEqual(0, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(1, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_multipleKeysInInstructionSetObject()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': [ 'x', 2 ], '*':[ 3 , 4] } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_valueInsteadOfParameterArray()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { '+': 3} ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedPrimitive()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", " { '*': [ 'y', { 'This is not a primitive': 3} ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullName()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew(null, "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullOperation()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", null, repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullRepository()
        {
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", null, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ErrorTest_nullPrimitives()
        {
            var repository = new InMemoryRepository();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { '+': [ 'x', 2 ] } ]}", repository, null);
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void ErrorTest_invalidJson()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", "This is not json", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_missingPrimitive()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { } ]} ", repository, primitives);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorTest_unexpectedNumberOfParametersForPrimitive()
        {
            var repository = new InMemoryRepository();
            var primitives = Primitive.LoadFromAssembly();
            InstructionSet.InsertNew("test", "{ '*': [ 'y', { '+': [ 'x' ] } ]}", repository, primitives);
        }

        [TestMethod]
        public void AllowArrayParameters()
        {
            var repository = new InMemoryRepository();
            var primitives = new Dictionary<string, IPrimitive>();
            primitives.Add("find", new TestPrimitive("find", 1, null));
            primitives.Add("*", new TestPrimitive("*", 2, null));
            InstructionSet.InsertNew("test", "{ 'find' : [ [ 1, 'x', { '*' : [ 2 , 3 ] }] ] }", repository, primitives);

            Assert.AreEqual(1, repository.Items.Count);

            InstructionSet instructionSet = repository.Items.Values.First();

            Assert.AreEqual(1, instructionSet.Parameters.Count, "Parameter count does not match");
            Assert.AreEqual(0, instructionSet.ChildInstructionSets.Count, "Child instruction set count does not match");
        }
    }
}
