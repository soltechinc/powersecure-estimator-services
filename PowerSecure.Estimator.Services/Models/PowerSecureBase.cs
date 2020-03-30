using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Models {
    public abstract class PowerSecureBase {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        public PowerSecureBase() { }

        public PowerSecureBase(string title) {
            Title = title;
        }
    }
}
