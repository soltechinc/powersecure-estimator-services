using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class Role
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("permissions")]
        public string[] Permissions { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
