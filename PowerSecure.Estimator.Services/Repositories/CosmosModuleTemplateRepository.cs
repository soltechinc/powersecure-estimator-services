﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosModuleTemplateRepository : IModuleTemplateRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosModuleTemplateRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseId");
            _collectionId = AppSettings.Get("moduleTemplatesCollectionId");
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["moduleTitle"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string moduleTitle, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleTitle) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == moduleTitle)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults)
            {
                foreach (ModuleTemplate ModuleTemplate in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: ModuleTemplate.Id), new RequestOptions { PartitionKey = new PartitionKey(ModuleTemplate.ModuleTitle) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var moduleTemplates = new List<ModuleTemplate>();

            bool reportFullObject = (queryParams.TryGetValue("object", out string value) && value.ToLower() == "full");
            while (query.HasMoreResults)
            {
                foreach (ModuleTemplate ModuleTemplate in await query.ExecuteNextAsync())
                {
                    if (!reportFullObject)
                    {
                        ModuleTemplate.Rest = null;
                    }
                    moduleTemplates.Add(ModuleTemplate);
                }
            }

            return moduleTemplates;
        }

        public async Task<object> Get(string moduleTitle, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]),
                    new RequestOptions { PartitionKey = new PartitionKey(moduleTitle) });
            }

            var query = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == moduleTitle)
                .AsDocumentQuery();

            var ModuleTemplates = new List<ModuleTemplate>();

            while (query.HasMoreResults)
            {
                foreach (ModuleTemplate ModuleTemplate in await query.ExecuteNextAsync())
                {
                    ModuleTemplates.Add(ModuleTemplate);
                }
            }

            return ModuleTemplates;
        }

        public async Task<int> Reset(JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var ModuleTemplates = new List<ModuleTemplate>();

            while (documentQuery.HasMoreResults)
            {
                foreach (ModuleTemplate ModuleTemplateDoc in await documentQuery.ExecuteNextAsync())
                {
                    ModuleTemplates.Add(ModuleTemplateDoc);
                }
            }

            foreach (var ModuleTemplate in ModuleTemplates)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: ModuleTemplate.Id), new RequestOptions { PartitionKey = new PartitionKey(ModuleTemplate.ModuleTitle) });
            }

            int count = 0;
            foreach (var child in jToken.Children())
            {
                if (child.Type != JTokenType.Object)
                {
                    continue;
                }

                await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), (JObject)child);

                count++;
            }

            return count;
        }
    }
}
