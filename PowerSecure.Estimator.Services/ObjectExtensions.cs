using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PowerSecure.Estimator.Services.ActionResults
{
    public static class ObjectExtensions
    {
        public static IActionResult ToOkObjectResult(this object obj, string message = "OK")
        {
            switch(obj)
            {
                case JObject jObj:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.OK);
                case IEnumerable e:
                    return ResultFromStatusCode(obj, e.Count(), message, HttpStatusCode.OK);
                default:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.OK);
            }
        }
        public static IActionResult ToServerErrorObjectResult(this object obj, string message = "Error")
        {
            switch (obj)
            {
                case JObject jObj:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.InternalServerError);
                case IEnumerable e:
                    return ResultFromStatusCode(obj, e.Count(), message, HttpStatusCode.InternalServerError);
                default:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.InternalServerError);
            }
        }

        private static IActionResult ResultFromStatusCode(object obj, int count, string message, HttpStatusCode httpStatusCode)
        {
            return new ObjectResult(JsonConvert.SerializeObject(new { Items = obj, Count = count, Message = message, Status = ((int)httpStatusCode) })) { StatusCode = (int)httpStatusCode };
        }

        private static int Count(this IEnumerable e)
        {
            int count = 0;
            foreach(var o in e)
            {
                count++;
            }
            return count;
        }
    }
}
