using Newtonsoft.Json;
using System;

namespace PowerSecure.Estimator.Services.Models
{
    public class Revision
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
