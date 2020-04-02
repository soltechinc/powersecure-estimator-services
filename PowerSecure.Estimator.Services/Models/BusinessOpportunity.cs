using Newtonsoft.Json;
using System;

namespace PowerSecure.Estimator.Services.Models {
    public class BusinessOpportunity {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ifsboNumber")]
        public string IFSBONumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("market")]
        public string VerticalMarket { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }
    }
}
