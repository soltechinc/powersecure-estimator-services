using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories {
    public interface IBusinessOpportunityLineItemRepository {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string id, IDictionary<string, string> queryParams);

        Task<object> Upsert(JObject document);

        Task<int> Delete(string id, IDictionary<string, string> queryParams);
    }
}
