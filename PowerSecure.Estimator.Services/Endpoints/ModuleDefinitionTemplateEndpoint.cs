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
    public static class ModuleDefinitionTemplateEndpoint
    {
        [FunctionName("GetModuleDefinitionTemplate")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moduleDefinitionTemplates/{estimateTemplateId}/{id}")] HttpRequest req,
            string estimateTemplateId,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetModuleDefinitionTemplate (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new ModuleDefinitionTemplateService(new CosmosEstimateTemplateRepository(dbClient)).Get(estimateTemplateId, id, queryParams);

                if (returnValue == null)
                {
                    return new object().ToNotFoundObjectResult(message: message);
                }

                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditModuleDefinitionTemplate")]
        public static async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "moduleDefinitionTemplates/{estimateTemplateId}")] HttpRequest req,
            string estimateTemplateId,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditModuleDefinitionTemplate");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new ModuleDefinitionTemplateService(new CosmosEstimateTemplateRepository(dbClient)).Upsert(estimateTemplateId, JObject.Parse(requestBody));

                if (returnValue == null)
                {
                    return new object().ToNotFoundObjectResult(message: message);
                }

                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteModuleDefinitionTemplate")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "moduleDefinitionTemplates/{estimateTemplateId}/{id}")] HttpRequest req,
            string estimateTemplateId,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - DeleteModuleDefinitionTemplate (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new ModuleDefinitionTemplateService(new CosmosEstimateTemplateRepository(dbClient)).Delete(estimateTemplateId, id, queryParams);

                if (returnValue == null)
                {
                    return new object().ToNotFoundObjectResult(message: message);
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
