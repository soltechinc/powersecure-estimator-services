using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services
{
    public static class ObjectExtensions
    {
        public static IActionResult ToOkObjectResult(this object obj)
        {
            switch(obj)
            {
                case IEnumerable e:
                    {
                        return new OkObjectResult(JsonConvert.SerializeObject(new { Items = obj, Message = "OK", Status = "200" }));
                    }
                default:
                    return new OkObjectResult(JsonConvert.SerializeObject(new { Items = new List<object> { obj }, Message = "OK", Status = "200" }));
            }
        }
    }
}
