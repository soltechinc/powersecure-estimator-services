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

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.List(queryParams),"OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.Get(id, queryParams),"OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _functionRepository.Upsert(document),"OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _functionRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
