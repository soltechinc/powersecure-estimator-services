using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleVendorQuoteService
    {
        private readonly IModuleVendorQuoteRepository _moduleVendorQuoteRepository;

        public ModuleVendorQuoteService(IModuleVendorQuoteRepository moduleVendorQuoteRepository)
        {
            _moduleVendorQuoteRepository = moduleVendorQuoteRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _moduleVendorQuoteRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _moduleVendorQuoteRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _moduleVendorQuoteRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _moduleVendorQuoteRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
