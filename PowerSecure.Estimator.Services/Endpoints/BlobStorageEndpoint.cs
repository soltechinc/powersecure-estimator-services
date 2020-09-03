using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PowerSecure.Estimator.Services.ActionResults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Services;
using Newtonsoft.Json;
using static PowerSecure.Estimator.Services.Shared.DTOs;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class BlobStorageEndpoint
    {
        [FunctionName("GetABS")]
        public static async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "abs")] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogDebug("Function called - GetABS");
                var abs = new ABSDTO();
                abs.blobStorageAccountName = AppSettings.Get("BlobStorageAccountName");
                abs.blobStorageConnectionString = AppSettings.Get("BlobStorageConnectionString");
                abs.blobStorageKey = AppSettings.Get("BlobStorageKey");
                abs.sasToken = AppSettings.Get("StorageAccountSASToken");
                string results = JsonConvert.SerializeObject(abs);
                return new JsonResult(results);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("UploadFile")]
        public static async Task<IActionResult> UploadFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "files")] HttpRequest req, ILogger log)
        {

            try
            {
                log.LogDebug("Function called - UploadFile");
                var dict = new Dictionary<string, object>();
                foreach(var pair in req.Form)
                {
                    dict.Add(pair.Key, pair.Value.ToArray());
                }

                var list = new List<byte[]>();
                if(req.Form.Files != null)
                {
                    foreach(var file in req.Form.Files)
                    {
                        using (MemoryStream mem = new MemoryStream())
                        {
                            file.CopyTo(mem);
                            list.Add(mem.ToArray());
                        }
                    }
                }

                log.LogInformation("Key/value data: " + JToken.FromObject(dict));
                log.LogInformation($"Found {list.Count} files with sizes {string.Join(",",list.Select(x => x.Length.ToString()))}");

                return "".ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
            /*
            try
            {
                log.LogDebug("Function called - UploadFile");
                var abs = new ABSDTO();
                abs.blobStorageAccountName = AppSettings.Get("BlobStorageAccountName");
                abs.blobStorageConnectionString = AppSettings.Get("BlobStorageConnectionString");
                abs.blobStorageKey = AppSettings.Get("BlobStorageKey");
                abs.sasToken = AppSettings.Get("StorageAccountSASToken");
                string results = JsonConvert.SerializeObject(abs);
                return new JsonResult(results);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }*/
        }
    }
}
