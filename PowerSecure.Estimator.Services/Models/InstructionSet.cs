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
        public InstructionSet(string id, string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, DateTime startDate, DateTime creationDate)
        {
            Id = id;
            Module = module;
            Name = name;
            Instructions = instructions;
            Parameters = new ReadOnlyCollection<string>((parameters ?? new string[] { }).ToList());
            ChildInstructionSets = new ReadOnlyCollection<string>((childInstructionSets ?? new string[] { }).ToList());
            StartDate = startDate;
            CreationDate = creationDate;
        }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("module")]
        public string Module { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonIgnore]
        public string Key => $"{Module}.{Name}";

        [JsonProperty("instructions")]
        public string Instructions { get; private set; }

        [JsonProperty("parameters")]
        public ReadOnlyCollection<string> Parameters { get; private set; }

        [JsonProperty("childinstructionsets")]
        public ReadOnlyCollection<string> ChildInstructionSets { get; private set; }

        [JsonProperty("startdate")]
        public DateTime StartDate { get; private set; }

        [JsonProperty("creationdate")]
        public DateTime CreationDate { get; private set; }

        public static InstructionSet Create(string id, string module, string name, string instructions, IEnumerable<string> parameters, IEnumerable<string> childInstructionSets, DateTime startDate, DateTime creationDate)
        {
            return new InstructionSet(id, module, name, instructions, parameters, childInstructionSets, startDate, creationDate);
        }

        public static InstructionSet FromFunction(Function f)
        {
            return new InstructionSet(f.Id, f.Module, f.Name, 
                f.Rest.ContainsKey("instructions") ? f.Rest["instructions"].ToString() : string.Empty,
                f.Rest.ContainsKey("parameters") ? ((JToken)f.Rest["parameters"]).Children().Select(o => o.ToString()) : new string[] { },
                f.Rest.ContainsKey("childinstructionsets") ? ((JToken)f.Rest["childinstructionsets"]).Children().Select(o => o.ToString()) : new string[] { },
                f.Rest.ContainsKey("startdate") ? DateTime.Parse(f.Rest["startdate"].ToString()) : DateTime.MinValue,
                f.Rest.ContainsKey("creationdate") ? DateTime.Parse(f.Rest["creationdate"].ToString()) : DateTime.MinValue);
        }
    }
}
