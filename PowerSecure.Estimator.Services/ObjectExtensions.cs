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
                        return ResultFromStatusCode(obj, message, httpStatusCode);
                default:
                    return ResultFromStatusCode(new List<object> { obj }, message, httpStatusCode);
            }
        }
        public static IActionResult ToServerErrorObjectResult(this object obj, string message = "Error", HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            switch (obj)
            {
                case IEnumerable e:
                    return ResultFromStatusCode(obj, message, httpStatusCode);
                default:
                    return ResultFromStatusCode(new List<object> { obj }, message, httpStatusCode);
            }
        }

        public static IActionResult ResultFromStatusCode(object obj, string message, HttpStatusCode httpStatusCode)
        {
            switch(httpStatusCode)
            {
                case HttpStatusCode.OK:
                        return new OkObjectResult(JsonConvert.SerializeObject(new { Items = obj, Message = message, Status = ((int)httpStatusCode) }));
                case HttpStatusCode.InternalServerError:
                        return new ObjectResult(JsonConvert.SerializeObject(new { Items = obj, Message = message, Status = ((int)httpStatusCode) }));
            }

            throw new Exception("unhandled status code");
        }
    }
}
