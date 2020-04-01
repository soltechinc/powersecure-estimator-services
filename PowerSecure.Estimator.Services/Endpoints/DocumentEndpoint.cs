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
                long contentLength = 0;
                for (int i = 0; blobList.Count > i; i++) {
                    var blob = blobList[i];
                    if (i == 0) { context.Response.Headers.Add("Content-Disposition", "Attachment; filename=" + blob.ToString()); }
                    await blob.DownloadToStreamAsync(memStream);
                    context.Response.ContentType = "application/json"; // blob.Properties.ContentType.ToString();
                    contentLength = contentLength + blob.Properties.Length;
                }
                context.Response.Headers.Add("Content-Length", contentLength.ToString());
                List<object> list = BlobStorageSettings.ConvertBlobListToFile(file);
                context.Response.Body.Write(memStream.ToArray());
                string results = JsonConvert.SerializeObject(list, Formatting.Indented);
                memStream.Flush();
                return new JsonResult(results);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
