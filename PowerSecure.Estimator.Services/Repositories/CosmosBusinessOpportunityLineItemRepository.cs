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
    public class CosmosBusinessOpportunityLineItemRepository : IBusinessOpportunityLineItemRepository {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly string _queryParamsKey;

        public CosmosBusinessOpportunityLineItemRepository(DocumentClient dbClient) {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("businessOpportunityLineItemCollectionId", EnvironmentVariableTarget.Process);
            _queryParamsKey = "ifsboliNumber";
        }


        public async Task<int> Delete(string ifsboliNumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey(_queryParamsKey)) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(ifsboliNumber) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunityLineItem>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.IFSBOLINumber == ifsboliNumber)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults) {
                foreach (BusinessOpportunityLineItem businessOpportunityLineItem in await query.ExecuteNextAsync()) {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: businessOpportunityLineItem.Id),
                        new RequestOptions { PartitionKey = new PartitionKey(businessOpportunityLineItem.IFSBOLINumber) }));
                }
            }

            return list.Count;
        }

        public async Task<object> Get(string ifsboliNumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey(_queryParamsKey)) {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(queryParams[_queryParamsKey]) });
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunityLineItem>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(i => i.IFSBOLINumber == ifsboliNumber)
                .AsDocumentQuery();

            var items = new List<BusinessOpportunityLineItem>();

            while (query.HasMoreResults) {
                foreach (BusinessOpportunityLineItem item in await query.ExecuteNextAsync()) {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> List(IDictionary<string, string> queryParams) {
            var query = _dbClient.CreateDocumentQuery<BusinessOpportunityLineItem>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var items = new List<BusinessOpportunityLineItem>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value)) {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults) {
                foreach (BusinessOpportunityLineItem item in await query.ExecuteNextAsync()) {
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
