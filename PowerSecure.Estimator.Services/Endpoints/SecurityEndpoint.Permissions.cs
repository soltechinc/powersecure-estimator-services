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
    public static partial class SecurityEndpoint
    {
        [FunctionName("ListPermissions")]
        public static async Task<IActionResult> ListPermissions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/permissions")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListPermissions");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new PermissionsService(new CosmosPermissionsRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetPermission")]
        public static async Task<IActionResult> GetPermission(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/permissions/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetPermission (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new PermissionsService(new CosmosPermissionsRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditPermission")]
        public static async Task<IActionResult> UpsertPermission(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/permissions")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditPermission");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new PermissionsService(new CosmosPermissionsRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeletePermission")]
        public static async Task<IActionResult> DeletePermission(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "security/permissions/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeletePermission (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new PermissionsService(new CosmosPermissionsRepository(dbClient)).Get(id, queryParams);
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
