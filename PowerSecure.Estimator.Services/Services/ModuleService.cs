using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleService
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            return await _moduleRepository.List(queryParams);
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            return await _moduleRepository.Get(id, queryParams);
        }

        public async Task<object> Upsert(JObject document)
        {
            return await _moduleRepository.Upsert(document);
        }
        
        public async Task<object> Delete(string id, IDictionary<string, string> queryParams)
        {
            return await _moduleRepository.Delete(id, queryParams);
        }
    }
}
