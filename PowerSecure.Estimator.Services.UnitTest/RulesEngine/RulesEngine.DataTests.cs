using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine
{
    [TestClass]
    public class RulesEngineDataTests
    {
        private static InMemoryReferenceDataRepository referenceDataRepository = new InMemoryReferenceDataRepository();
        private static InMemoryInstructionSetRepository instructionSetRepository = new InMemoryInstructionSetRepository();
        private static IDictionary<string, IPrimitive> primitives = Primitive.Load();

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            referenceDataRepository.Load(@".\Resources\General Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\General Reference Formula Table.csv", primitives);

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void LoadRepository()
        {
            Assert.AreNotEqual(0, primitives.Count);
            Assert.AreNotEqual(0, referenceDataRepository.Items.Count);
            Assert.AreNotEqual(0, instructionSetRepository.Items.Count);

            Trace.WriteLine("Reference Data");
            referenceDataRepository.Items.ForEach(o => Trace.WriteLine(JsonConvert.SerializeObject(o, Formatting.Indented)));

            Trace.WriteLine("Instruction Sets");
            instructionSetRepository.Items.Select(o => o.Value).ForEach(set => Trace.WriteLine(JsonConvert.SerializeObject(set, Formatting.Indented)));
        }

        [TestMethod]
        public void RunAllInstructionSets()
        {
            var dataSheet = new Dictionary<string, object>();

            instructionSetRepository.Items.Select(o => o.Value).ForEach(set => dataSheet.Add(set.Name, null));

            dataSheet["DesiredInstallRate"] = 3.5m;

            var rulesEngine = new Components.RulesEngine.RulesEngine();

            rulesEngine.EvaluateDataSheet(dataSheet, primitives, instructionSetRepository, referenceDataRepository);
        }
    }
}
