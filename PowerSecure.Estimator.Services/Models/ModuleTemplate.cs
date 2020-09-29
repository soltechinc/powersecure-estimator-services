using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class ModuleTemplate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("moduleTitle")]
        public string ModuleTitle { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
