using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class BusinessOpportunityService {
        private readonly IBusinessOpportunityRepository _businessOpportunityRepository;

        public BusinessOpportunityService(IBusinessOpportunityRepository businessOpportunityRepository)
        {
            _businessOpportunityRepository = businessOpportunityRepository;
        }

        public async Task<(object,string)> List(IDictionary<string, string> queryParams)
        {
            return (await _businessOpportunityRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _businessOpportunityRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _businessOpportunityRepository.Upsert(document), "OK");
        }
        
        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _businessOpportunityRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
