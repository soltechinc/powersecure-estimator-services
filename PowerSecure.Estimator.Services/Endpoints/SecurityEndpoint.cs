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

namespace PowerSecure.Estimator.Services.Endpoints {
    public static class SecurityEndpoint {
        [FunctionName("GetUser")]
        public static async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/users/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug($"Function called - GetUser (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosAuthorizedUserRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditUser")]
        public static async Task<IActionResult> UpsertUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/users")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug("Function called - EditUser");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new SecurityService(new CosmosAuthorizedUserRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ListRoles")]
        public static async Task<IActionResult> List(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/roles")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log) {
            try {
                log.LogDebug("Function called - ListRoles");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("GetRole")]
        public static async Task<IActionResult> GetRole(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/roles/{id}")] HttpRequest req,
        string id,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log) {
            try {
                log.LogDebug($"Function called - GetRole (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditRole")]
        public static async Task<IActionResult> UpsertRole(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/roles")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log) {
            try {
                log.LogDebug("Function called - EditRole");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            } catch (Exception ex) {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
