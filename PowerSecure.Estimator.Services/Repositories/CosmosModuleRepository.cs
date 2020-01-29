﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosModuleRepository : IModuleRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosModuleRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("modulesCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["moduleTitle"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<object> Delete(string moduleTitle, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleTitle) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == moduleTitle)
                .AsDocumentQuery();

            var list = new List<ResourceResponse<Document>>();
            while (query.HasMoreResults)
            {
                foreach (Module m in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: m.Id), new RequestOptions { PartitionKey = new PartitionKey(m.ModuleTitle) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List()
        {
            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var documentIds = new List<Module>();

            while (query.HasMoreResults)
            {
                foreach (Module m in await query.ExecuteNextAsync())
                {
                    documentIds.Add(m);
                }
            }

            return documentIds;
        }

        public async Task<object> Get(string moduleTitle, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleTitle) });
            }

            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == moduleTitle)
                .AsDocumentQuery();

            var documents = new List<Document>();

            while (query.HasMoreResults)
            {
                foreach (Module m in await query.ExecuteNextAsync())
                {
                    documents.Add(await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: m.Id), new RequestOptions { PartitionKey = new PartitionKey(m.ModuleTitle) }));
                }
            }

            return documents;
        }
    }
}