using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PowerSecure.Estimator.Services.Services;
using Microsoft.Azure.Documents.Client;
using PowerSecure.Estimator.Services.Repositories;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class ModuleEndpoint
    {
        [FunctionName("ListModules")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "modules")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace("Function called - ListModules");
            
            return (await new ModuleService(new CosmosModuleRepository(dbClient)).List()).ToOkObjectResult();
        }

        [FunctionName("CreateNewModule")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "modules")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace("Function called - CreateNewModule");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var response = await new ModuleService(new CosmosModuleRepository(dbClient)).Upsert(JObject.Parse(requestBody));

            return response.ToOkObjectResult();
        }

        [FunctionName("DeleteModule")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "modules/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace("Function called - DeleteModule");

            var response = await new ModuleService(new CosmosModuleRepository(dbClient)).Delete(id);

            return response.ToOkObjectResult();
        }
    }
}
