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
        private IDictionary<string, IPrimitive> primitives = Primitive.Load();

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void LoadRepository()
        {
            var referenceDataRepository = new InMemoryReferenceDataRepository();
            var instructionSetRepository = new InMemoryInstructionSetRepository();
            referenceDataRepository.Load(@".\Resources\General Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\General Reference Formula Table.csv", primitives);
            referenceDataRepository.Load(@".\Resources\PEC Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\PEC Reference Formula Table.csv", primitives);

            Assert.AreNotEqual(0, primitives.Count);
            Assert.AreNotEqual(0, referenceDataRepository.Items.Count);
            Assert.AreNotEqual(0, instructionSetRepository.Items.Count);

            Trace.WriteLine("Reference Data");
            referenceDataRepository.Items.ForEach(o => Trace.WriteLine(JsonConvert.SerializeObject(o, Formatting.Indented)));

            Trace.WriteLine("Instruction Sets");
            instructionSetRepository.Items.Select(o => o.Value).ForEach(set => Trace.WriteLine(JsonConvert.SerializeObject(set, Formatting.Indented)));
        }

        [TestMethod]
        public void RunAllGeneralInstructionSets()
        {
            var referenceDataRepository = new InMemoryReferenceDataRepository();
            var instructionSetRepository = new InMemoryInstructionSetRepository();
            referenceDataRepository.Load(@".\Resources\General Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\General Reference Formula Table.csv", primitives);

            var dataSheet = new Dictionary<string, object>();
            
            instructionSetRepository.Items.Select(o => o.Value.First()).ForEach(set => {
                dataSheet.Add(set.Key, null);
                Trace.WriteLine(set.Key);
            });

            dataSheet["DesiredInstallRate"] = 3.5m;
            dataSheet["USState"] = "GEORGIA";

            var rulesEngine = new Components.RulesEngine.RulesEngine();

            rulesEngine.EvaluateDataSheet(dataSheet, DateTime.Now, primitives, instructionSetRepository, referenceDataRepository);
        }

        [TestMethod]
        public void RunAllPECInstructionSets()
        {
            var referenceDataRepository = new InMemoryReferenceDataRepository();
            var instructionSetRepository = new InMemoryInstructionSetRepository();
            referenceDataRepository.Load(@".\Resources\General Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\General Reference Formula Table.csv", primitives);
            referenceDataRepository.Load(@".\Resources\PEC Reference Fact Table.csv");
            instructionSetRepository.Load(@".\Resources\PEC Reference Formula Table.csv", primitives);

            var dataSheet = new Dictionary<string, object>();

            instructionSetRepository.Items.Select(o => o.Value.First()).ForEach(set => {
                dataSheet.Add(set.Key, null); 
                Trace.WriteLine(set.Key);
            });

            dataSheet["DesiredInstallRate"] = 3.5m;
            dataSheet["USState"] = "GEORGIA";
            dataSheet["SwitchgearType"] = "MV";
            dataSheet["quantityofidenticalswitchgears"] = 2m;
            dataSheet["numberofswitchgearsections"] = 3m;
            dataSheet["includepecpad"] = true;
            dataSheet["materialsalestaxexemptionmultiplier"] = 1m;
            dataSheet["includepecrigging"] = true;
            dataSheet["projecttype"] = "Everything";
            dataSheet["hvacsizetonnage"] = 0m;
            dataSheet["hvacmanufacturingunittonnage"] = 35m;
            dataSheet["exteriorlightsquantity"] = 1m;
            dataSheet["interiorlightsquantity"] = 2m;
            dataSheet["exterioroutletsquantity"] = 3m;
            dataSheet["interioroutletsquantity"] = 4m;
            dataSheet["interiorswitchesquantity"] = 5m;
            dataSheet["photoeyequantity"] = 6m;
            dataSheet["36inchdoorsingledoorquantity"] = 7m;
            dataSheet["72inchdoubledoorquantity"] = 8m;
            dataSheet["25kvahousetransformerquantity"] = 9m;
            dataSheet["100a277to480vhousepanelhphquantity"] = 10m;
            dataSheet["100a120to240vhousepanelhplquantity"] = 11m;
            dataSheet["facpquantity"] = 12m;
            dataSheet["smokedetectorquantity"] = 13m;
            dataSheet["pullstationquantity"] = 14m;
            dataSheet["hornstrobequantity"] = 15m;
            dataSheet["hotdipgalvanizationquantity"] = 16m;
            dataSheet["hvacquantity"] = 17m;
            dataSheet["includepecinstallation"] = true;
            dataSheet["pecequipmentinstallationlaborhoursarray"] = new object[] { 4m, 3m, 7m };
            dataSheet["pecinitialequipmentcostarray"] = new object[] { 5m, 3m, 7m };
            dataSheet["pecinitialequipmenttaxarray"] = new object[] { 4m, 2m, 7m };
            dataSheet["pecinstallationcostarray"] = new object[] { 4m, 3m, 9m };
            dataSheet["pecpadsinstallationcostarray"] = new object[] { 4m, 8m, 7m };
            dataSheet["pecrigginginstallationcostarray"] = new object[] { 2m, 3m, 7m };
            dataSheet["pecpowerfablaborhoursarray"] = new object[] { 9m, 3m, 7m };
            dataSheet["controlhouselength"] = 2m;
            dataSheet["controlhousewidth"] = 4m;
            dataSheet["controlhouseheight"] = 7m;
            dataSheet["controlhousequantity"] = 9m;
            dataSheet["includesalestax"] = true;
            dataSheet["materialaftertaxmultiplier"] = 1m;
            dataSheet["includecontrolhouseinstallation"] = true;
            dataSheet["includecontrolhousepads"] = true;
            dataSheet["includecontrolhouserigging"] = true;

            var rulesEngine = new Components.RulesEngine.RulesEngine();

            rulesEngine.EvaluateDataSheet(dataSheet, DateTime.Now, primitives, instructionSetRepository, referenceDataRepository);
        }
    }
}
