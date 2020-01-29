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
    public static class FunctionEndpoint
    {
        [FunctionName("ListFunctions")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "functions")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogTrace("Function called - ListFunctions");

                var queryParams = req.GetQueryParameterDictionary();

                return (await new FunctionService(new CosmosFunctionRepository(dbClient)).List(queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("GetFunction")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "functions/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogTrace($"Function called - GetFunction (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                return (await new FunctionService(new CosmosFunctionRepository(dbClient)).Get(id, queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("EditFunction")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "functions")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogTrace("Function called - EditFunction");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
                return (await new FunctionService(new CosmosFunctionRepository(dbClient)).Upsert(JObject.Parse(requestBody))).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }

        [FunctionName("DeleteFunction")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "functions/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogTrace($"Function called - DeleteFunction (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();
            
                return (await new FunctionService(new CosmosFunctionRepository(dbClient)).Delete(id, queryParams)).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }
    }
}
