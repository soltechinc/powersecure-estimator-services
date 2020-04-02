using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Models {
    public class AuthorizedUser {
        
        [JsonProperty("boliNumber")]
        public string BOLINumber { get; set; }

        [JsonProperty("ifsUserID")]
        public string IFSUserID { get; set; }

        [JsonProperty("authorizedUserFirstName")]
        public string AuthorizedUserFirstName { get; set; }

        [JsonProperty("authorizedUserLastName")]
        public string AuthorizedUserLastName { get; set; }

        public string InferredIFSUserID {
            get {
                return $"{AuthorizedUserFirstName.Substring(0, 1).ToUpper()}{AuthorizedUserLastName.ToUpper()}";
            }
        }
    }
}
