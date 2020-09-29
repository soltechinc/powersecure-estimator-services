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
    public class CosmosPermissionsRepository : ISecurityRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosPermissionsRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseAdminId");
            _collectionId = AppSettings.Get("permissionCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["ifsboNumber"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }


        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<Permissions>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<Permissions>();

            while (query.HasMoreResults)
            {
                foreach (Permissions item in await query.ExecuteNextAsync())
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

            var query = _dbClient.CreateDocumentQuery<Permissions>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.Id == id)
                .AsDocumentQuery();

            var items = new List<Permissions>();

            while (query.HasMoreResults)
            {
                foreach (Permissions item in await query.ExecuteNextAsync())
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
