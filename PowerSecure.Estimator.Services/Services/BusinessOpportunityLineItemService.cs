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
    public class BusinessOpportunityLineItemService {
        private readonly IBusinessOpportunityLineItemRepository _businessOpportunityLineItemRepository;

        public BusinessOpportunityLineItemService(IBusinessOpportunityLineItemRepository businessOpportunityLineItemRepository)
        {
            _businessOpportunityLineItemRepository = businessOpportunityLineItemRepository;
        }

        public async Task<(object,string)> List(IDictionary<string, string> queryParams)
        {
            return (await _businessOpportunityLineItemRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _businessOpportunityLineItemRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _businessOpportunityLineItemRepository.Upsert(document), "OK");
        }
        
        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _businessOpportunityLineItemRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
