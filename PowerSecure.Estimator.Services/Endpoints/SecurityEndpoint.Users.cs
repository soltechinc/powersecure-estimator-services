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
        [FunctionName("ListUsers")]
        public static async Task<IActionResult> ListUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/users")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListUsers");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new UserService(new CosmosUserRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
        
        [FunctionName("GetUser")]
        public static async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/users/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetUser (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new UserService(new CosmosUserRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditUser")]
        public static async Task<IActionResult> UpsertUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/users")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditUser");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new UserService(new CosmosUserRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteUser")]
        public static async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "security/users/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteUser (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new UserService(new CosmosUserRepository(dbClient)).Delete(id, queryParams);
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
