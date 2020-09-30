using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosRoleRepository : IRoleRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosRoleRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseAdminId");
            _collectionId = AppSettings.Get("roleCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["id"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<Role>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<Role>();

            bool reportFullObject = (queryParams.TryGetValue("object", out string value) && value.ToLower() == "full");
            while (query.HasMoreResults)
            {
                foreach (Role item in await query.ExecuteNextAsync())
                {
                    if (!reportFullObject)
                    {
                        item.Rest = null;
                    }
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

        public async Task<int> Delete(string id)
        {
            await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(id) });
            return 1;
        }

        public async Task<int> Reset(JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<Role>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = false })
                .AsDocumentQuery();

            var ids = new List<string>();

            while (documentQuery.HasMoreResults)
            {
                foreach (Role item in await documentQuery.ExecuteNextAsync())
                {
                    ids.Add(item.Id);
                }
            }

            foreach (var id in ids)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(id) });
            }

            int count = 0;
            foreach (var child in jToken.Children())
            {
                if (child.Type != JTokenType.Object)
                {
                    continue;
                }

                var item = child.ToObject<Role>();

                await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), item);

                count++;
            }

            return count;
        }
    }
}
