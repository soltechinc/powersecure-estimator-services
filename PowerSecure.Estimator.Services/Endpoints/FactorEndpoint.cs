using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using PowerSecure.Estimator.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class FactorEndpoint
    {
        [FunctionName("ListFactors")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace("Function called - ListFactors");
            
            return (await new FactorService(new CosmosFactorRepository(dbClient)).List(req.GetQueryParameterDictionary())).ToOkObjectResult();
        }

        [FunctionName("GetFactor")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace($"Function called - GetFactor (Id: {id})");

            return (await new FactorService(new CosmosFactorRepository(dbClient)).Get(id, req.GetQueryParameterDictionary())).ToOkObjectResult();
        }

        [FunctionName("EditFactor")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "factors")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace("Function called - EditFactor");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var response = await new FactorService(new CosmosFactorRepository(dbClient)).Upsert(JObject.Parse(requestBody));

            return response.ToOkObjectResult();
        }

        [FunctionName("DeleteFactor")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "factors/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            log.LogTrace($"Function called - DeleteFactor (Id: {id})");

            var response = await new FactorService(new CosmosFactorRepository(dbClient)).Delete(id, req.GetQueryParameterDictionary());

            return response.ToOkObjectResult();
        }
    }
}
