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
        public static Models.File file = new Models.File();

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
            
            //-- change later begin --//

            var containerName = "file-uploads";
            string storageConnection = "BlobEndpoint=https://powersecureestimatorblob.blob.core.windows.net/;TableEndpoint=https://powersecureestimatorblob.table.core.windows.net/;SharedAccessSignature=sv=2019-02-02&ss=b&srt=sco&sp=rwdlac&se=2099-03-25T03:59:59Z&st=2020-03-24T15:10:40Z&spr=https&sig=j53pQUYsB7IU7GXexc4cm3kAknx9BDC8n%2BdNrUczacs%3D";

            //-- change later end --//

            //string storageConnection = AppSettings.Get("BlobStorageConnectionString");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference("blob-test-file.docx");
            MemoryStream memStream = new MemoryStream();
            //blobClient.ListBlobsSegmentedAsync();
            await blockBlob.DownloadToStreamAsync(memStream);
            context.Response.ContentType = blockBlob.Properties.ContentType.ToString();
            context.Response.Headers.Add("Content-Disposition", "Attachment; filename=" + blockBlob.ToString());
            context.Response.Headers.Add("Content-Length", blockBlob.Properties.Length.ToString());

            //-- Beginning of hardcoded test code --//
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

            //-- end of hardcoded test code --//

            string results = JsonConvert.SerializeObject(file);
            //memStream.Flush();
            return new JsonResult(results);
        }
    }
}
