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
    public class CosmosBusinessOpportunityRepository : IBusinessOpportunityRepository {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly string _queryParamsKey;

        public CosmosBusinessOpportunityRepository(DocumentClient dbClient) {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("businessOpportunityCollectionId", EnvironmentVariableTarget.Process);
            _queryParamsKey = "ifsboNumber";
        }

        public async Task<int> Delete(string ifsBONumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey(_queryParamsKey)) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(ifsBONumber) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.IFSBONumber == ifsBONumber)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults) {
                foreach (BusinessOpportunity businessOpportunity in await query.ExecuteNextAsync()) {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: businessOpportunity.Id), 
                        new RequestOptions { PartitionKey = new PartitionKey(businessOpportunity.IFSBONumber) }));
                }
            }

            return list.Count;
        }

        public async Task<object> Get(string ifsBONumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey(_queryParamsKey)) {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), 
                    new RequestOptions { PartitionKey = new PartitionKey(queryParams[_queryParamsKey]) });
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(i => i.IFSBONumber == ifsBONumber)
                .AsDocumentQuery();

            var businessOpportunities = new List<BusinessOpportunity>();

            while (query.HasMoreResults) {
                foreach (BusinessOpportunity businessOpportunity in await query.ExecuteNextAsync()) {
                    businessOpportunities.Add(businessOpportunity);
                }
            }

            return businessOpportunities;
        }

        public async Task<object> List(IDictionary<string, string> queryParams) {
            var query = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<BusinessOpportunity>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value)) {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults) {
                foreach (BusinessOpportunity item in await query.ExecuteNextAsync()) {
                    if (!reportFullObject) {
                        //item.Rest = null;
                    }
                    items.Add(item);
                }
            }
            return items;
        }

        public async Task<object> Upsert(JObject document) {
            if (document.ContainsKey("id")) {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["module"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }
    }
}
