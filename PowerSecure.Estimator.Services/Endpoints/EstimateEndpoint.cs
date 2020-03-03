using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.ActionResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Services;
using PowerSecure.Estimator.Services.Repositories;

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

                (object returnValue, string message) = await new EstimateService(new CosmosFunctionRepository(dbClient), new CosmosFactorRepository(dbClient), log).Evaluate(JObject.Parse(requestBody));
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
