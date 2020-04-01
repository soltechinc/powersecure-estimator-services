using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PowerSecure.Estimator.Services.Services;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.ActionResults;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Endpoints {
    public class DocumentEndpoint {
        public static Models.File file = new Models.File();
 
        [FunctionName("PostDocument")]
        public static async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "document")] HttpRequest req, ILogger log) {

            return null;
        }


        [FunctionName("GetDocuments")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req, ILogger log) {
            try {
                HttpContext context = req.HttpContext;
                MemoryStream memStream = new MemoryStream();
                BlobStorageSettings.GetAllFiles();
                var blobList = BlobStorageSettings.BlobList;
                for (int i = 0; blobList.Count > i; i++) {
                    var blob = blobList[i];
                    await blob.DownloadToStreamAsync(memStream);
                }
                List<object> list = BlobStorageSettings.ConvertBlobListToFile(file);
                string results = JsonConvert.SerializeObject(list);
                return new JsonResult(results);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
