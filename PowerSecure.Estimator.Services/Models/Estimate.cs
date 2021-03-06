﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class Estimate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("boName")]
        public string BOName { get; set; }

        [JsonProperty("revisionDate")]
        public string RevisionDate { get; set; }

        [JsonProperty("boliName")]
        public string BOLIName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("boliDescription")]
        public string BOLIDescription { get; set; }

        [JsonProperty("versionName")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("boNumber")]
        public string BONumber { get; set; }

        [JsonProperty("boliNumber")]
        public string BOLINumber { get; set; }

        [JsonProperty("estimateNumber")]
        public string EstimateNumber { get; set; }

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

        [JsonProperty("outsideEquipmentPercent")]
        public string OutsideEquipmentPercent { get; set; }

        [JsonProperty("softCostPercent")]
        public string SoftCostPercent { get; set; }

        [JsonProperty("desiredRateForInstall")]
        public string DesiredRateForInstall { get; set; }

        [JsonProperty("boliId")]
        public string BOLIId { get; set; }

        [JsonProperty("includedModules")]
        public List<ModuleDefinition> Modules { get; set; }
    }
}
