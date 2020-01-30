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
            try
            {
                log.LogDebug("Function called - ListModules");

                var queryParams = req.GetQueryParameterDictionary();

                return (await new ModuleService(new CosmosModuleRepository(dbClient)).List(queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("GetModule")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "modules/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetModule (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                return (await new ModuleService(new CosmosModuleRepository(dbClient)).Get(id, queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("EditModule")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "modules")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditModule");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                return (await new ModuleService(new CosmosModuleRepository(dbClient)).Upsert(JObject.Parse(requestBody))).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("DeleteModule")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "modules/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            { 
                log.LogDebug($"Function called - DeleteModule (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                return (await new ModuleService(new CosmosModuleRepository(dbClient)).Delete(id, queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }
    }
}
