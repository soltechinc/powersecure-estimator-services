using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class Permissions
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("permission")]
        public string Permission { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
