using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.UnitTest.Services
{
    [TestClass]
    public class EstimateServiceTests
    {
        public void HappyPathTest_evaluate()
        {
            var estimateService = new EstimateService(null,null);

            JObject inputFromUi = JObject.Parse(File.ReadAllText(@".\Resources\module_evaluate_sample_json1.json"));

            var retValue = Task.Run(async () => await estimateService.Evaluate(inputFromUi)).GetAwaiter().GetResult();
        }
    }
}
