﻿using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Models
{
    public class ModuleVendorQuote
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("moduleId")]
        public string ModuleId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
