using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.ActionResults;
using PowerSecure.Estimator.Services.Repositories;
using PowerSecure.Estimator.Services.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class BusinessOpportunityEndpoint
    {
        [FunctionName("ListBusinessOpportunities")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunities")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListBusinessOpportunities");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityService(new CosmosBusinessOpportunityRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetBusinessOpportunity")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunities/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetBusinessOpportunity (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityService(new CosmosBusinessOpportunityRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditBusinessOpportunity")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "businessOpportunities")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditBusinessOpportunity");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new BusinessOpportunityService(new CosmosBusinessOpportunityRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteBusinessOpportunity")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BusinessOpportunities/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteBusinessOpportunity (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityService(new CosmosBusinessOpportunityRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportBusinessOpportunities")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunities/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ImportBusinessOpportunities (Env: {env})");

                (object returnValue, string message) = await new BusinessOpportunityService(new CosmosBusinessOpportunityRepository(dbClient)).Import(env);

                if (returnValue == null)
                {
                    return new object().ToServerErrorObjectResult(message: message);
                }

                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
