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
using System.Text;
using System.Net.Http;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class BlobStorageEndpoint
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> UploadFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "files")] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogDebug("Function called - UploadFile");
                var dict = new Dictionary<string, object>();
                foreach (var pair in req.Form)
                {
                    dict.Add(pair.Key.ToLower(), pair.Value.ToArray());
                }

                string path = ((string[])dict["path"])[0];

                var list = new List<string>();
                if(req.Form.Files != null)
                {
                    if(req.Form.Files.Count > 1)
                    {
                        int fileCount = 0;
                        foreach (var file in req.Form.Files)
                        {
                            using (MemoryStream mem = new MemoryStream())
                            {
                                file.CopyTo(mem);
                                mem.Position = 0;
                                var retValue = await new BlobStorageService().UploadFile(mem, $"{path}-{fileCount}", log);
                                list.Add(retValue.Item1.ToString());
                            }
                            fileCount++;
                        }
                    }
                    else
                    {
                        foreach (var file in req.Form.Files)
                        {
                            using (MemoryStream mem = new MemoryStream())
                            {
                                file.CopyTo(mem);
                                mem.Position = 0;
                                var retValue = await new BlobStorageService().UploadFile(mem, path, log);
                                list.Add(retValue.Item1.ToString());
                            }
                        }
                    }
                }

                return list.Select(x => new Dictionary<string, object> { ["Path"] = x, ["Url"] = $"api/files/{x}" }).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DownloadFile")]
        public static async Task<IActionResult> DownloadFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{path}")] HttpRequest req, string path, ILogger log)
        {
            try
            {
                log.LogDebug("Function called - DownloadFile");
                var retValue = await new BlobStorageService().DownloadFile(path, log);
                return new FileStreamResult((Stream)retValue.Item1, "application/octet-stream"); ;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
