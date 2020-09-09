using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerSecure.Estimator.Services.Models
{
    public class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("blobGuid")]
        public Guid BlobGuid { get; set; } // unsure if we need

        [JsonProperty("includeInProposal")]
        public bool IncludeInProposal { get; set; }

        [JsonProperty("uploadedBy")]
        public string UploadedBy { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("updated")]
        public DateTime? Updated { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("moduleId")]
        public string ModuleId { get; set; }
    }
}
