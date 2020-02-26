using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.Services
{
    [TestClass]
    public class EstimateServiceTests
    {
        [TestMethod]
        public void HappyPathTest_singleInstruction()
        {
            var estimateService = new EstimateService(null);

            JObject inputFromUi = JObject.Parse(File.ReadAllText(@".\Resources\module_evaluate_sample_json1.json"));

            estimateService.Evaluate(inputFromUi);
        }
    }
}
