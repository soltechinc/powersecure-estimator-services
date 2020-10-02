using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class EstimateTemplate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("includedModules")]
        public List<ModuleDefinition> Modules { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
