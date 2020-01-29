using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Models
{
    public class Function
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
