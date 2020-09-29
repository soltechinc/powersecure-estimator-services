using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class Factor
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("returnattribute")]
        public string ReturnAttribute { get; set; }

        [JsonProperty("returnvalue")]
        public string ReturnValue { get; set; }

        [JsonProperty("startdate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("creationdate")]
        public DateTime CreationDate { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }
}
