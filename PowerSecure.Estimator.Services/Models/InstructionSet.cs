using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Models
{
    public class InstructionSet : IInstructionSet
    {
        public InstructionSet(string id, string module, string name, string instructions, DateTime startDate, DateTime creationDate, JObject uiJson)
        {
            Id = id;
            Module = module;
            Name = name;
            Instructions = instructions;
            StartDate = startDate;
            CreationDate = creationDate;
            UiJson = uiJson;
        }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("module")]
        public string Module { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("instructions")]
        public string Instructions { get; private set; }

        [JsonProperty("startdate")]
        public DateTime StartDate { get; private set; }

        [JsonProperty("creationdate")]
        public DateTime CreationDate { get; private set; }

        [JsonProperty("uijson")]
        public JObject UiJson { get; private set; }
    }
}
