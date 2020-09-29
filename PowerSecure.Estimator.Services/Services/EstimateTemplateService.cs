using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateTemplateService
    {
        private readonly IEstimateTemplateRepository _estimateTemplateRepository;

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository)
        {
            _estimateTemplateRepository = estimateTemplateRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _estimateTemplateRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _estimateTemplateRepository.Get(id), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _estimateTemplateRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _estimateTemplateRepository.Delete(id);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
