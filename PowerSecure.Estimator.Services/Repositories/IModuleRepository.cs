using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public interface IModuleRepository
    {
        Task<object> List();

        Task<object> Upsert(JObject document);

        Task<object> Delete(string id);
    }
}
