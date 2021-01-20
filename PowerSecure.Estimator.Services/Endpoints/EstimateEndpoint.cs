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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class EstimateEndpoint
    {
        [FunctionName("EvaluateEstimate")]
        public static async Task<IActionResult> Evaluate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "estimates/evaluate")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EvaluateEstimate");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"Request Body: {requestBody}");

                (object returnValue, string message) = await new EstimateService(new CosmosFunctionRepository(dbClient), new CosmosFactorRepository(dbClient), new CosmosEstimateRepository(dbClient), new CosmosBusinessOpportunityLineItemRepository(dbClient)).Evaluate(JObject.Parse(requestBody), log);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ListEstimates")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "estimates")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListEstimates");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("GetEstimate")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "estimates/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetEstimate (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                if (id.ToLower() == "lookup")
                {
                    (object returnValue, string message) = await new EstimateService(new CosmosFunctionRepository(dbClient), new CosmosFactorRepository(dbClient), new CosmosEstimateRepository(dbClient), new CosmosBusinessOpportunityLineItemRepository(dbClient)).Lookup(queryParams, log);

                    if (returnValue == null)
                    {
                        return new object().ToNotFoundObjectResult();
                    }

                    return returnValue.ToOkObjectResult(message: message);
                }
                else
                { 
                    (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).Get(id, queryParams);
                    return returnValue.ToOkObjectResult(message: message);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("EditEstimate")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "estimates")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditEstimate");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("CloneVersion")]
        public static async Task<IActionResult> CloneVersion(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "estimates/clone/version")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug("Function called - CloneVersion");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).CloneVersion(JObject.Parse(requestBody), log);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("CloneRevision")]
        public static async Task<IActionResult> CloneRevision(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "estimates/clone/revision")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug("Function called - CloneRevision");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).CloneRevision(JObject.Parse(requestBody), log);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("DeleteEstimate")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "estimates/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteEstimate (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).Delete(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ExportSummary")]
        public static async Task<IActionResult> ExportSummary(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "estimates/export/summary")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ExportSummary");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"Request Body: {requestBody}");

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).ExportSummary(JObject.Parse(requestBody), log);

                if (returnValue == null)
                {
                    return new object().ToServerErrorObjectResult(message: message);
                }

                string path = returnValue.ToString();
                return new List<Dictionary<string, object>> { new Dictionary<string, object> { ["Path"] = path, ["Url"] = $"/files/{path}?filename=*.xlsx" } }.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ExportOwnedAsset")]
        public static async Task<IActionResult> ExportOwnedAsset(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "estimates/export/ownedasset")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - ExportSummary");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"Request Body: {requestBody}");

                (object returnValue, string message) = await new EstimateService(new CosmosEstimateRepository(dbClient)).ExportOwnedAsset(JObject.Parse(requestBody), log);

                if (returnValue == null)
                {
                    return new object().ToServerErrorObjectResult(message: message);
                }

                string path = returnValue.ToString();
                return new List<Dictionary<string, object>> { new Dictionary<string, object> { ["Path"] = path, ["Url"] = $"/files/{path}?filename=*.xlsx" } }.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
