using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class FunctionService
    {
        private readonly IFunctionRepository _functionRepository;

        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            return await _functionRepository.List(queryParams);
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            return await _functionRepository.Get(id, queryParams);
        }

        public async Task<object> Upsert(JObject document)
        {
            return await _functionRepository.Upsert(document);
        }

        public async Task<object> Delete(string id, IDictionary<string, string> queryParams)
        {
            return await _functionRepository.Delete(id, queryParams);
        }
    }
}
