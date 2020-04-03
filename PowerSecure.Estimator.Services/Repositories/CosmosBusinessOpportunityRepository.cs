﻿using Microsoft.Azure.Documents;
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

        public CosmosBusinessOpportunityRepository(DocumentClient dbClient) {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("businessOpportunityCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document) {
            if (document.ContainsKey("id")) {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["ifsboNumber"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string ifsboNumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey("id")) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(ifsboNumber) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.IFSBONumber == ifsboNumber)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults) {
                foreach (BusinessOpportunity item in await query.ExecuteNextAsync()) {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: item.Id), new RequestOptions { PartitionKey = new PartitionKey(item.IFSBONumber) }));
                }
            }

            return list.Count;
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
                        //module.Rest = null;
                    }
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<object> Get(string ifsboNumber, IDictionary<string, string> queryParams) {
            if (queryParams.ContainsKey("id")) {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(ifsboNumber) });
            }

            var query = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(i => i.IFSBONumber == ifsboNumber)
                .AsDocumentQuery();

            var items = new List<BusinessOpportunity>();

            while (query.HasMoreResults) {
                foreach (BusinessOpportunity item in await query.ExecuteNextAsync()) {
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<int> Reset(JToken jToken) {
            var documentQuery = _dbClient.CreateDocumentQuery<BusinessOpportunity>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var items = new List<BusinessOpportunity>();

            while (documentQuery.HasMoreResults) {
                foreach (BusinessOpportunity item in await documentQuery.ExecuteNextAsync()) {
                    items.Add(item);
                }
            }

            foreach (var item in items) {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: item.Id), new RequestOptions { PartitionKey = new PartitionKey(item.IFSBONumber) });
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
