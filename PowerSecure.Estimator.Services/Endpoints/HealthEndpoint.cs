using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PowerSecure.Estimator.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.ActionResults;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class HealthEndpoint
    {
        [FunctionName("CheckProperties")]
        public static IActionResult CheckProperties(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/properties")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - CheckProperties");

                (object returnValue, string message) = new HealthService().CheckProperties();
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
