﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories {
    public interface IEstimateRepository {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string id, IDictionary<string, string> queryParams);

        Task<object> Upsert(JObject document);

        Task<object> Clone(JObject document);

        Task<int> Delete(string id, IDictionary<string, string> queryParams);

        Task<int> Reset(string module, JToken jToken);

        Task<int> Reset(JToken jToken);
    }
}
