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

namespace PowerSecure.Estimator.Services.Endpoints
{
    public class DocumentEndpoint {
        public static HttpClient httpClient;
        public static Models.File file;

        [FunctionName("PostDocument")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "documents")] HttpRequest req,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.docupilot.app/documents/create/4b869acb/61c19454");
            request.Content = new StringContent("{\n    \"client\": {\n        \"name\": \"SOLTECH-Test\",\n        \"title\": \"SOLTECH-Test\",\n        \"company\": \"SOLTECH\",\n        \"address\": \"950 East Paces Ferry Rd NE #2400\",\n        \"state\": \"GA\",\n        \"country\": \"United States\",\n        \"pincode\": \"30326\"\n    }\n}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await httpClient.SendAsync(request);
            return response;
        }


        [FunctionName("GetDocuments")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req, ILogger log) {
            HttpContext context = req.HttpContext;
            var containerName = "file-uploads";      // change later                            
            string storageConnection = AppSettings.Get("BlobStorageConnectionString");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference("blob-test-file.docx");
            MemoryStream memStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memStream);
            context.Response.ContentType = blockBlob.Properties.ContentType.ToString();
            context.Response.Headers.Add("Content-Disposition", "Attachment; filename=" + blockBlob.ToString());
            context.Response.Headers.Add("Content-Length", blockBlob.Properties.Length.ToString());

            //-- Beginning of test code --//
            string description = "";
            if (blockBlob.Metadata.Count > 0) {
                foreach (var val in blockBlob.Metadata.Values) {
                    description = val;
                }
            }
            context.Response.Body.Write(memStream.ToArray());

            file.Name = blockBlob.Name;
            file.Uri = blockBlob.Uri.ToString();
            file.Description = description;
            

            //-- end of test code --//

            string results = JsonConvert.SerializeObject(file);
            //memStream.Flush();
            return new JsonResult(results);
        }
    }
}
