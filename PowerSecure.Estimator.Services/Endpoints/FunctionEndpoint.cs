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
                log.LogDebug("Function called - ListFunctions");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
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
                log.LogDebug($"Function called - GetFunction (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
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
                log.LogDebug("Function called - EditFunction");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).Upsert(JObject.Parse(requestBody));

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

        [FunctionName("DeleteFunction")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "functions/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteFunction (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportFunctions")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "functions/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ImportFunctions (Env: {env})");

                var queryParams = req.GetQueryParameterDictionary();

                if (!queryParams.ContainsKey("module"))
                {
                    return new object().ToServerErrorObjectResult(message: "Query params do not contain module name");
                }

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).Import(env, queryParams["module"]);

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

        [FunctionName("EditFunctionFromUi")]
        public static async Task<IActionResult> UpsertFromUi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "functions/ui")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditFunctionFromUi");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation(requestBody);

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).UpsertFromUi(requestBody);

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

        [FunctionName("GetFunctionFromUi")]
        public static async Task<IActionResult> GetFromUi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "functions/ui/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - GetFunctionFromUi");

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).GetFromUi(id);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ListPrimitives")]
        public static async Task<IActionResult> ListPrimitives(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "primitives")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListPrimitives");

                (object returnValue, string message) = await new FunctionService(new CosmosFunctionRepository(dbClient), log).ListPrimitives();
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
