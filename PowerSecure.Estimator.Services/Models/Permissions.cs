using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Models
{
    public class Permissions
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("list")]
        public string[] List { get; set; }
    }
}
