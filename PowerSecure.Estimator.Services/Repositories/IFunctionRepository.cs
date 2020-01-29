using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public interface IFunctionRepository
    {
        Task<object> List(IDictionary<string, string> queryParams);

        Task<object> Get(string id, IDictionary<string, string> queryParams);

        Task<object> Upsert(JObject document);

        Task<object> Delete(string id, IDictionary<string, string> queryParams);
    }
}
