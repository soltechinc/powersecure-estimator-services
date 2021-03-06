﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Models
{
    public class ModuleDefinition
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("moduleId")]
        public string ModuleId { get; set; }

        [JsonProperty("moduleTitle")]
        public string ModuleTitle { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("materialCost")]
        public string MaterialCost { get; set; }

        [JsonProperty("laborCost")]
        public string LaborCost { get; set; }

        [JsonProperty("totalCost")]
        public string TotalCost { get; set; }

        [JsonProperty("materialUseTax")]
        public string MaterialUseTax { get; set; }

        [JsonProperty("totalCostWithTax")]
        public string TotalCostWithTax { get; set; }

        [JsonProperty("sellPrice")]
        public string SellPrice { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Rest { get; set; }
    }
}
