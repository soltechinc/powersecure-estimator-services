using Newtonsoft.Json;
using System;

namespace PowerSecure.Estimator.Services.Models {
    public class BusinessOpportunity {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ifsboNumber")]
        public string IFSBONumber { get; set; }

        [JsonProperty("opportunityName")]
        public string OpportunityName { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("dateDue")]
        public DateTime DateDue { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }
    }
}
