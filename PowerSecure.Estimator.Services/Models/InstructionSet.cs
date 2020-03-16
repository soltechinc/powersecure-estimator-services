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
        public InstructionSet(string id, string module, string name, string instructions, DateTime startDate, DateTime creationDate)
        {
            Id = id;
            Module = module;
            Name = name;
            Instructions = instructions;
            StartDate = startDate;
            CreationDate = creationDate;
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

        public static InstructionSet Create(string id, string module, string name, string instructions, DateTime startDate, DateTime creationDate)
        {
            return new InstructionSet(id, module, name, instructions, startDate, creationDate);
        }

        public static InstructionSet FromFunction(Function f)
        {
            return new InstructionSet(f.Id, f.Module, f.Name, 
                f.Rest.ContainsKey("instructions") ? f.Rest["instructions"].ToString() : string.Empty,
                f.Rest.ContainsKey("startdate") ? DateTime.Parse(f.Rest["startdate"].ToString()) : DateTime.MinValue,
                f.Rest.ContainsKey("creationdate") ? DateTime.Parse(f.Rest["creationdate"].ToString()) : DateTime.MinValue);
        }
    }
}
