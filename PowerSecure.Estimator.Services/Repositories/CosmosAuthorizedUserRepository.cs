using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories {
    public class CosmosAuthorizedUserRepository : ISecurityRepository {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosAuthorizedUserRepository(DocumentClient dbClient) {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseAdminId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("userCollectionId", EnvironmentVariableTarget.Process);
        }

        private async Task<JObject> SetDefaultRole(JObject document) {
            JToken result;
            document.TryGetValue("role", out result);
            if (result.Type != JTokenType.String) {
                string role = "Estimator";
                document["role"] = role;
            }

            return document;
        }

        public async Task<object> Upsert(JObject document) {
            document = await SetDefaultRole(document);
            if (document.ContainsKey("id")) {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["id"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }


        public async Task<object> Get(string id, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey("id")) {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(id) });
            }

            var query = _dbClient.CreateDocumentQuery<AuthorizedUser>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.Id == id)
                .AsDocumentQuery();

            var items = new List<AuthorizedUser>();

            while (query.HasMoreResults) {
                foreach (AuthorizedUser item in await query.ExecuteNextAsync()) {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> List(IDictionary<string, string> queryParams) {
            var query = _dbClient.CreateDocumentQuery<AuthorizedUser>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<AuthorizedUser>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value)) {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults) {
                foreach (AuthorizedUser item in await query.ExecuteNextAsync()) {
                    if (!reportFullObject) {
                        //item.Rest = null;
                    }
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
