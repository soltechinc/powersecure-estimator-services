using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Models
{
    public class Role
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public object[] Name { get; set; }
    }
}
