using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class EstimateTemplate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
