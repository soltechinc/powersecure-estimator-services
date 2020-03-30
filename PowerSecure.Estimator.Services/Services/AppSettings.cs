using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Services {
    public static class AppSettings {        
        public static string Get(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }

}
