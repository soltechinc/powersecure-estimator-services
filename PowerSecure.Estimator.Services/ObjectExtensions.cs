using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PowerSecure.Estimator.Services
{
    public static class ObjectExtensions
    {
        public static IActionResult ToOkObjectResult(this object obj, string message = "OK", HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            switch(obj)
            {
                case IEnumerable e:
                    {
                        return new OkObjectResult(JsonConvert.SerializeObject(new { Items = obj, Message = message, Status = ((int)httpStatusCode) }));
                    }
                default:
                    return new OkObjectResult(JsonConvert.SerializeObject(new { Items = new List<object> { obj }, Message = message, Status = ((int)httpStatusCode) }));
            }
        }
    }
}
