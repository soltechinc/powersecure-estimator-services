using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public interface IFactorRepository
    {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string id, IDictionary<string, string> queryParams);

        Task<object> Upsert(JObject document);

        Task<int> Delete(string id, IDictionary<string, string> queryParams);

        Task<int> Reset(string module, JToken jToken);

        Task<object> Lookup(IDictionary<string, string> queryParams);
    }
}
