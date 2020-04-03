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
using PowerSecure.Estimator.Services.ActionResults;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class BusinessOpportunityLineItemEndpoint {
        [FunctionName("ListBusinessOpportunityLineItems")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug("Function called - ListBusinessOpportunityLineItems");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - GetBusinessOpportunityLineItem (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "businessOpportunityLineItems")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug("Function called - EditBusinessOpportunityLineItem");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteBusinessOpportunityLineItem")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BusinessOpportunityLineItem/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - DeleteBusinessOpportunityLineItem (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportBusinessOpportunityLineItems")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "businessOpportunityLineItems/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - ImportBusinessOpportunities (Env: {env})");

                (object returnValue, string message) = await new BusinessOpportunityLineItemService(new CosmosBusinessOpportunityLineItemRepository(dbClient)).Import(env);

                if (returnValue == null) {
                    return new object().ToServerErrorObjectResult(message: message);
                }

                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
