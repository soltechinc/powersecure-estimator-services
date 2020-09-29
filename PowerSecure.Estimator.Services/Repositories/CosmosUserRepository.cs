using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using User = PowerSecure.Estimator.Services.Models.User;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosUserRepository : IUserRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosUserRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseAdminId");
            _collectionId = AppSettings.Get("userCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (!document.TryGetValue("role", out JToken result) || result.Type != JTokenType.String)
            {
                document.Add("role", JToken.FromObject("Estimator"));
            }

            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["id"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }


        public async Task<object> Get(string id)
        {
            return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id),
                new RequestOptions { PartitionKey = new PartitionKey(id) });
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<User>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<Models.User>();

            while (query.HasMoreResults)
            {
                foreach (Models.User item in await query.ExecuteNextAsync())
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public async Task<int> Delete(string id)
        {
            await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(id) });
            return 1;
        }
    }
}
