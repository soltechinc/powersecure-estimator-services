using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosEstimateTemplateRepository : IEstimateTemplateRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosEstimateTemplateRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseId");
            _collectionId = AppSettings.Get("estimateTemplateCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["id"].ToString()) });
            }
            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string id)
        {
            await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(id) });
            return 1;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<Estimate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<EstimateTemplate>();

            while (query.HasMoreResults)
            {
                foreach (EstimateTemplate item in await query.ExecuteNextAsync())
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> Get(string id)
        {
            return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id),
                new RequestOptions { PartitionKey = new PartitionKey(id) });
        }
    }
}