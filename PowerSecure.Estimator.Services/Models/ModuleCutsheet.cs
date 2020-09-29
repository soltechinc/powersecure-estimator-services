using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Models
{
    public class ModuleCutsheet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("configuration")]
        public string Configuration { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("moduleTitle")]
        public string ModuleTitle { get; set; }
    }
}
