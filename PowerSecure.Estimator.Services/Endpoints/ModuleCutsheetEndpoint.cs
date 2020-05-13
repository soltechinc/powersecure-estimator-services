using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PowerSecure.Estimator.Services.Services;
using PowerSecure.Estimator.Services.ActionResults;
using Microsoft.Azure.Documents.Client;
using PowerSecure.Estimator.Services.Repositories;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.Endpoints {
    public class ModuleCutsheetEndpoint {
        [FunctionName("ListModuleCutsheets")]
        public static async Task<IActionResult> List(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moduleCutsheets")] HttpRequest req,
             [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
             ILogger log) {
            try {
                log.LogDebug("Function called - ListModuleCutsheets");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new ModuleCutsheetService(new CosmosModuleVendorQuoteRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetModuleCutsheet")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moduleCutsheets/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - GetModuleCutsheets (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new ModuleCutsheetService(new CosmosModuleCutsheetRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditModuleCutsheet")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "moduleCutsheets")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug("Function called - EditModuleCutsheet");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new ModuleCutsheetService(new CosmosModuleCutsheetRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteModuleCutsheet")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "moduleCutsheets/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - DeleteModuleCutsheet (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new ModuleCutsheetService(new CosmosModuleCutsheetRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportModuleCutsheets")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moduleCutsheets/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - ImportModulesCutsheets (Env: {env})");

                (object returnValue, string message) = await new ModuleCutsheetService(new CosmosModuleCutsheetRepository(dbClient)).Import(env);

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
