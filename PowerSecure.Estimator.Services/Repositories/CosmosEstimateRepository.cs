using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories {
    public class CosmosEstimateRepository : IEstimateRepository {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly string _childCollectionId;

        public CosmosEstimateRepository(DocumentClient dbClient) {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("estimateCollectionId", EnvironmentVariableTarget.Process);
        }


        public async Task<object> Clone(JObject document) {
            //if (document.ContainsKey("id")) {
            //    return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["boliNumber"].ToString()) });
            //}

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        private async Task<JObject> UpdateNumber(JObject document) {
            JToken result;
            string estimateNumber = "1";
            document.TryGetValue("estimateNumber", out result);
            if (result.Type == JTokenType.String) {
                estimateNumber = JTokenExtension.IncrementString(result.ToString());                
            }
            document["estimateNumber"] = estimateNumber;
            return document;
        }

        public async Task<object> Upsert(JObject document) {
            //if (document.ContainsKey("id")) {
                document = await UpdateNumber(document);
              //return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["boliNumber"].ToString()) });
            //}

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string boli, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey("id")) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(boli) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<Estimate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.BOLINumber == boli)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults) {
                foreach (Estimate item in await query.ExecuteNextAsync()) {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: item.Id), new RequestOptions { PartitionKey = new PartitionKey(item.BOLINumber) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams) {
            var query = _dbClient.CreateDocumentQuery<Estimate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<Estimate>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value)) {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults) {
                foreach (Estimate item in await query.ExecuteNextAsync()) {
                    if (!reportFullObject) {
                        //module.Rest = null;
                    }
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> Get(string boli, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey("id")) {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(boli) });
            }

            var query = _dbClient.CreateDocumentQuery<Estimate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.BOLINumber == boli)
                .AsDocumentQuery();

            var items = new List<Estimate>();

            while (query.HasMoreResults) {
                foreach (Estimate item in await query.ExecuteNextAsync()) {
                    var module = new List<ModuleDefinition>();
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<int> Reset(JToken jToken) {
            var documentQuery = _dbClient.CreateDocumentQuery<Estimate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var items = new List<Estimate>();

            while (documentQuery.HasMoreResults) {
                foreach (Estimate item in await documentQuery.ExecuteNextAsync()) {
                    items.Add(item);
                }
            }

            foreach (var item in items) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: item.Id), new RequestOptions { PartitionKey = new PartitionKey(item.Title) });
            }

            int count = 0;
            foreach (var child in jToken.Children()) {
                if (child.Type != JTokenType.Object) {
                    continue;
                }

                await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), (JObject)child);

                count++;
            }

            return count;
        }
    }
}