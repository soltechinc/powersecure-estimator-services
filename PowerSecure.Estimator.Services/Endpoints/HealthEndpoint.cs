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

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class HealthEndpoint
    {
        [FunctionName("CheckProperties")]
        public static async Task<IActionResult> CheckProperties(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/properties")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogTrace("Function called - ListModules");

                return (await new HealthService().CheckProperties()).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return ex.ToServerErrorObjectResult(ex.Message);
            }
        }
    }
}
