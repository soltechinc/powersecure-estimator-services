using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public interface IEstimateTemplateRepository
    {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string id);

        Task<object> Upsert(JObject document);

        Task<int> Delete(string id);
    }
}
