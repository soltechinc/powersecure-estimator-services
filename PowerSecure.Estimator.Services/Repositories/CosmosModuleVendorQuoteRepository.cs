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
    public class CosmosModuleVendorQuoteRepository : IModuleVendorQuoteRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosModuleVendorQuoteRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseAdminId");
            _collectionId = AppSettings.Get("vendorQuoteCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["id"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(id) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<ModuleVendorQuote>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.Id == id)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults)
            {
                foreach (ModuleVendorQuote item in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: item.Id), new RequestOptions { PartitionKey = new PartitionKey(item.Id) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<ModuleVendorQuote>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<ModuleVendorQuote>();

            while (query.HasMoreResults)
            {
                foreach (ModuleVendorQuote item in await query.ExecuteNextAsync())
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(id) });
            }

            var query = _dbClient.CreateDocumentQuery<ModuleVendorQuote>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.Id == id)
                .AsDocumentQuery();

            var items = new List<ModuleVendorQuote>();

            while (query.HasMoreResults)
            {
                foreach (ModuleVendorQuote item in await query.ExecuteNextAsync())
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
