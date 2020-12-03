using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace PowerSecure.Estimator.Services.ActionResults
{
    public static class ObjectExtensions
    {
        public static IActionResult ToOkObjectResult(this object obj, string message = "OK")
        {
            switch (obj)
            {
                case JObject jObj:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.OK);
                case string s:
                    return ResultFromStatusCode(new List<object> { s }, 1, message, HttpStatusCode.OK);
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
                case string s:
                    return ResultFromStatusCode(new List<object> { s }, 1, message, HttpStatusCode.InternalServerError);
                case IEnumerable e:
                    return ResultFromStatusCode(obj, e.Count(), message, HttpStatusCode.InternalServerError);
                default:
                    return ResultFromStatusCode(new List<object> { obj }, 1, message, HttpStatusCode.InternalServerError);
            }
        }

        public static IActionResult ToNotFoundObjectResult(this object obj, string message = "Not Found")
        {
            return ResultFromStatusCode(new List<object>(), 0, message, HttpStatusCode.NotFound);
        }

        private static IActionResult ResultFromStatusCode(object obj, int count, string message, HttpStatusCode httpStatusCode)
        {
            return new ObjectResult(JsonConvert.SerializeObject(new { Items = obj, Count = count, Message = message, Status = ((int)httpStatusCode) })) { StatusCode = (int)httpStatusCode };
        }

        private static int Count(this IEnumerable e)
        {
            int count = 0;
            foreach (var o in e)
            {
                count++;
            }
            return count;
        }
    }
}


namespace PowerSecure.Estimator.Services
{
    public static class ObjectExtensions
    { 
        public static string UnwrapString(this object obj)
        {
            if(obj == null)
            {
                return string.Empty;
            }

            return UnwrapString(obj.ToString());
        }

        public static string UnwrapString(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (str.StartsWith("$"))
            {
                return str.Substring(1);
            }
            return str;
        }
    }
}
