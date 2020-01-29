using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class FactorService
    {
        private readonly IFactorRepository _factorRepository;

        public FactorService(IFactorRepository factorRepository)
        {
            _factorRepository = factorRepository;
        }

        public async Task<object> List(IDictionary<string,string> queryParams)
        {
            return await _factorRepository.List(queryParams);
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            return await _factorRepository.Get(id, queryParams);
        }

        public async Task<object> Upsert(JObject document)
        {
            //Add key and hash functionality
            document["key"] = string.Join('-', document["module"], document["returnAttribute"]);
            document["hash"] = document.ToString();
            return await _factorRepository.Upsert(document);
        }

        public async Task<object> Delete(string id, IDictionary<string, string> queryParams)
        {
            return await _factorRepository.Delete(id, queryParams);
        }
    }
}
