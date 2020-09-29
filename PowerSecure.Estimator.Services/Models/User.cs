using Newtonsoft.Json;

namespace PowerSecure.Estimator.Services.Models
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }
}
