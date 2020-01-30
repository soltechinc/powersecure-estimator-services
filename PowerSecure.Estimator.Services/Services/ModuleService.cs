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

        public async Task<(object,string)> List(IDictionary<string, string> queryParams)
        {
            return (await _moduleRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _moduleRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _moduleRepository.Upsert(document), "OK");
        }
        
        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _moduleRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
