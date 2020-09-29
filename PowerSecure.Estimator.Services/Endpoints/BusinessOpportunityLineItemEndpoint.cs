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
    public static class BusinessOpportunityLineItemEndpoint
    {
        [FunctionName("ListBusinessOpportunityLineItems")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListBusinessOpportunityLineItems");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetBusinessOpportunityLineItem (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "businessOpportunityLineItems")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditBusinessOpportunityLineItem");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BusinessOpportunityLineItem/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteBusinessOpportunityLineItem (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportBusinessOpportunityLineItems")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ImportBusinessOpportunities (Env: {env})");

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Import(env);

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

        [FunctionName("ListStates")]
        public static async Task<IActionResult> ListStates(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/data/states")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ListStates");

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).ListStates();

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

        [FunctionName("ListVerticalMarkets")]
        public static async Task<IActionResult> ListVerticalMarkets(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/data/verticalMarkets")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ListVerticalMarkets");

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).ListVerticalMarkets();

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

        [FunctionName("ListProjectTypes")]
        public static async Task<IActionResult> ListProjectTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/data/projectTypes")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ListProjectTypes");

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).ListProjectTypes();

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
