using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerSecure.Estimator.Services.Models {
    public class File : PowerSecureBase {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("blobGuid")]
        public Guid BlobGuid { get; set; }

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
        public Guid ModuleId { get; set; }

        public File() : base() {

        }

        public async void CreateFileInDirectory(IFormFile file) {
            try {
                //check if file exist
                var fileName = Path.GetFileName(file.Name);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"www", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(fileStream);
                }

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public File(string title) : base(title) {

        }        
    }
}
