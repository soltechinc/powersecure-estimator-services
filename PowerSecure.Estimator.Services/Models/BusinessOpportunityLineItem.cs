using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class BusinessOpportunityLineItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ifsboNumber")]
        public string IFSBONumber { get; set; }

        [JsonProperty("ifsboliNumber")]
        public string IFSBOLINumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("fav")]
        public bool Favorite { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("authorizedUsers")]
        public List<User> AuthorizedUsers { get; set; }
    }
}
