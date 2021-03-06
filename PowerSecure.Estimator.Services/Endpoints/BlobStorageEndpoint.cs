﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PowerSecure.Estimator.Services.ActionResults;
using PowerSecure.Estimator.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class BlobStorageEndpoint
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> UploadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "files")] HttpRequest req, 
            ILogger log)
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
                if (req.Form.Files != null)
                {
                    if (req.Form.Files.Count > 1)
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

                return list.Select(x => new Dictionary<string, object> { ["Path"] = x, ["Url"] = $"/files/{x}" }).ToOkObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DownloadFile")]
        public static async Task<IActionResult> DownloadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{path}")] HttpRequest req, 
            string path, 
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - DownloadFile");

                var queryParams = req.GetQueryParameterDictionary();

                (object stream, string message) = await new BlobStorageService().DownloadFile(path, log);

                if (stream != null)
                {
                    if (queryParams.ContainsKey("filename"))
                    {
                        string filename = queryParams["filename"];
                        if (Path.GetFileNameWithoutExtension(filename) == "*")
                        {
                            filename = $"{path}{Path.GetExtension(filename)}";
                        }
                        return new FileStreamResult((Stream)stream, "application/octet-stream")
                        {
                            FileDownloadName = filename
                        };
                    }
                    else
                    {
                        return new FileStreamResult((Stream)stream, "application/octet-stream");
                    }
                }
                return new object().ToNotFoundObjectResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("DeleteFile")]
        public static async Task<IActionResult> DeleteFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "files/{path}")] HttpRequest req, 
            string path, 
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - DeleteFile");
                (object returnValue, string message) = await new BlobStorageService().DeleteFile(path, log);
                if (returnValue == null)
                {
                    return new object[0].ToOkObjectResult(message: message);
                }
                return new List<Dictionary<string, object>> { new Dictionary<string, object> { ["Path"] = path, ["Url"] = $"/files/{path}" } }.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
