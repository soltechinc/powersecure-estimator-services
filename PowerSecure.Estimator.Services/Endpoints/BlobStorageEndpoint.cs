using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.ActionResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Services;
using PowerSecure.Estimator.Services.Repositories;
using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Endpoints {
    public static class BlobStorageEndpoint {
        
    [FunctionName("GetABS")]
        public static async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "abs")] HttpRequest req, ILogger log
        ) {

            try {
                log.LogDebug("Function called - GetABS");
                var abs = new ABSDTO();
                abs.blobStorageAccountName = AppSettings.Get("BlobStorageAccountName");
                abs.blobStorageConnectionString = AppSettings.Get("BlobStorageConnectionString");
                abs.blobStorageKey = AppSettings.Get("BlobStorageKey");
                string results = JsonConvert.SerializeObject(abs);
                return new JsonResult(results);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
