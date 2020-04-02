using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models {
    public class Estimate {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // unsure we need

        [JsonProperty("boNumber")]
        public string BONumber { get; set; }

        [JsonProperty("boliNumber")]
        public string BOLINumber { get; set; }

        [JsonProperty("estimateNumber")]
        public string EstimateNumber { get; set; } // unsure we need

        [JsonProperty("revisionNumber")]
        public string RevisionNumber { get; set; }

        [JsonProperty("versionNumber")]
        public string VersionNumber { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("projectType")]
        public string ProjectType { get; set; }

        [JsonProperty("modules")]
        public List<Module> Modules { get; set; }
    }
}
