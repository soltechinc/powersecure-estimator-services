﻿using Microsoft.AspNetCore.Http;
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
    public static class FactorEndpoint
    {
        [FunctionName("ListFactors")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListFactors");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetFactor")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetFactor (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                if (id.ToLower() == "lookup")
                {
                    (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Lookup(queryParams);

                    if (returnValue == null)
                    {
                        return new object().ToNotFoundObjectResult();
                    }

                    return returnValue.ToOkObjectResult(message: message);
                }
                else
                {
                    (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Get(id, queryParams);
                    return returnValue.ToOkObjectResult(message: message);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditFactorList")]
        public static async Task<IActionResult> UpsertList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "factorlist")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditFactor");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).UpsertList(JObject.Parse(requestBody.ToLower()));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditFactor")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "factors")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditFactor");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Upsert(JObject.Parse(requestBody.ToLower()));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteFactor")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "factors/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteFactor (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ImportFactors")]
        public static async Task<IActionResult> Import(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors/import/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ImportFactors (Env: {env})");

                var queryParams = req.GetQueryParameterDictionary();

                if (!queryParams.ContainsKey("module"))
                {
                    return new object().ToServerErrorObjectResult(message: "Query params do not contain module name");
                }

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Import(env, queryParams["module"]);

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

        [FunctionName("ResetFactors")]
        public static async Task<IActionResult> Reset(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "factors/reset/{env}")] HttpRequest req,
            string env,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ResetFactors (Env: {env})");

                var queryParams = req.GetQueryParameterDictionary();

                if (!queryParams.ContainsKey("module"))
                {
                    return new object().ToServerErrorObjectResult(message: "Query params do not contain module name");
                }

                (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Reset(env, queryParams["module"]);

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
