using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories {
    public interface IBusinessOpportunityRepository {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string ifsboNumber, IDictionary<string, string> queryParams);

        Task<object> Upsert(JObject document);

        Task<int> Delete(string ifsboNumber, IDictionary<string, string> queryParams);
    }
}
