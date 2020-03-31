using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PowerSecure.Estimator.Services.ActionResults;
using System;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Services;
using Newtonsoft.Json;
using static PowerSecure.Estimator.Services.Shared.DTOs;

namespace PowerSecure.Estimator.Services.Endpoints {
    public static class BlobStorageEndpoint {
        
    [FunctionName("GetABS")]
        public static async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "abs")] HttpRequest req, ILogger log) {
            try {
                log.LogDebug("Function called - GetABS");
                var abs = new ABSDTO();
                abs.blobStorageAccountName = AppSettings.Get("BlobStorageAccountName");
                abs.blobStorageConnectionString = AppSettings.Get("BlobStorageConnectionString");
                abs.blobStorageKey = AppSettings.Get("BlobStorageKey");
                abs.sasToken = AppSettings.Get("StorageAccountSASToken");
                string results = JsonConvert.SerializeObject(abs);
                return new JsonResult(results);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
