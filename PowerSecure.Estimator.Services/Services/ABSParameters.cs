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

    public class ABSDTO {
        public ABSDTO() { }

        public string blobStorageAccountName { get; set; }
        public string blobStorageConnectionString { get; set; }
        public string blobStorageKey { get; set; }
        public string containerName { get; set; }
        public string sasToken { get; set; }
    }
}
