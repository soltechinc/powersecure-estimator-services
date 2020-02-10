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

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class DocumentEndpoint
    {
        public static HttpClient httpClient = new HttpClient();

        [FunctionName("DocumentEndpoint")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "documents")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.docupilot.app/documents/create/4b869acb/61c19454");
            request.Content = new StringContent("{\n    \"client\": {\n        \"name\": \"SOLTECH-Test\",\n        \"title\": \"SOLTECH-Test\",\n        \"company\": \"SOLTECH\",\n        \"address\": \"950 East Paces Ferry Rd NE #2400\",\n        \"state\": \"GA\",\n        \"country\": \"United States\",\n        \"pincode\": \"30326\"\n    }\n}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await httpClient.SendAsync(request);
            return response;
        }
    }
}
