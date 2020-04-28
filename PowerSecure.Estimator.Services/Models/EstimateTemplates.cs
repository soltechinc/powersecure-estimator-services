using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models {
    public class EstimateTemplates {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("versionName")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("boNumber")]
        public string BONumber { get; set; }

        [JsonProperty("boliNumber")]
        public string BOLINumber { get; set; }

        [JsonProperty("revisionNumber")]
        public string RevisionNumber { get; set; }

        [JsonProperty("versionNumber")]
        public string VersionNumber { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("projectType")]
        public string ProjectType { get; set; }

        [JsonProperty("estimatePrice")]
        public string EstimatePrice { get; set; }

        [JsonProperty("includedModules")]
        public List<ModuleDefinition> Modules { get; set; }
    }
}
