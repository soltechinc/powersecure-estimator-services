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

namespace PowerSecure.Estimator.Services.Endpoints {
    public static class BlobStorageEndpoint {

        [FunctionName("GetABS")]
        public static async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "abs")] HttpRequest req,
        string id,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient, 
        ILogger log
        ) {

            //var queryParams = req.GetQueryParameterDictionary();

           // (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Get(id, queryParams);
            var obj = new object();
            var test = ABSParameters.Get("BlobStorageAccountName");
            var test2 = "";

            log.LogDebug($"Function called - GetFactor (Id: {id})");

            var queryParams = req.GetQueryParameterDictionary();

            (object returnValue, string message) = await new FactorService(new CosmosFactorRepository(dbClient)).Get(id, queryParams);
            return returnValue.ToOkObjectResult(message: message);

          //  return test;
        }
    }
}
