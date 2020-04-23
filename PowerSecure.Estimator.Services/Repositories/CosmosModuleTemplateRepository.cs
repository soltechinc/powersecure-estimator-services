using Microsoft.Azure.Documents;
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
    public class CosmosModuleTemplateRepository : IModuleTemplateRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosModuleTemplateRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("moduleTemplatesCollectionId", EnvironmentVariableTarget.Process);
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
                foreach (ModuleTemplate moduleTemplate in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: moduleTemplate.Id), new RequestOptions { PartitionKey = new PartitionKey(moduleTemplate.ModuleTitle) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var moduleTemplates = new List<ModuleTemplate>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value))
            {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults)
            {
                foreach (ModuleTemplate moduleTemplate in await query.ExecuteNextAsync())
                {
                    if(!reportFullObject)
                    {
                         moduleTemplate.Rest = null;
                    }
                    moduleTemplates.Add(moduleTemplate);
                }
            }

            return moduleTemplates;
        }

        public async Task<object> Get(string moduleTitle, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleTitle) });
            }

            var query = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == moduleTitle)
                .AsDocumentQuery();

            var moduleTemplates = new List<ModuleTemplate>();

            while (query.HasMoreResults)
            {
                foreach (ModuleTemplate moduleTemplate in await query.ExecuteNextAsync())
                {
                    moduleTemplates.Add(moduleTemplate);
                }
            }

            return moduleTemplates;
        }

        public async Task<int> Reset(JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<ModuleTemplate>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var moduleTemplates = new List<ModuleTemplate>();

            while (documentQuery.HasMoreResults)
            {
                foreach (ModuleTemplate moduleDoc in await documentQuery.ExecuteNextAsync())
                {
                    moduleTemplates.Add(moduleDoc);
                }
            }

            foreach (var moduleTemplate in moduleTemplates)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: moduleTemplate.Id), new RequestOptions { PartitionKey = new PartitionKey(moduleTemplate.ModuleTitle) });
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
