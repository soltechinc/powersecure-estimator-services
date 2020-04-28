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
    public class CosmosModuleDefinitionRepository : IModuleDefinitionRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosModuleDefinitionRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("moduleDefinitionsCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["moduleName"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string moduleName, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleName) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<ModuleDefinition>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleName == moduleName)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults)
            {
                foreach (ModuleDefinition moduleDefinition in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: moduleDefinition.Id), new RequestOptions { PartitionKey = new PartitionKey(moduleDefinition.Id) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            var query = _dbClient.CreateDocumentQuery<ModuleDefinition>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            var moduleDefinitions = new List<ModuleDefinition>();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value))
            {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (query.HasMoreResults)
            {
                foreach (ModuleDefinition moduleDefinition in await query.ExecuteNextAsync())
                {
                    if(!reportFullObject)
                    {
                         moduleDefinition.Rest = null;
                    }
                    moduleDefinitions.Add(moduleDefinition);
                }
            }

            return moduleDefinitions;
        }

        public async Task<object> Get(string moduleName, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: queryParams["id"]), new RequestOptions { PartitionKey = new PartitionKey(moduleName) });
            }

            var query = _dbClient.CreateDocumentQuery<ModuleDefinition>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleName == moduleName)
                .AsDocumentQuery();

            var moduleDefinitions = new List<ModuleDefinition>();

            while (query.HasMoreResults)
            {
                foreach (ModuleDefinition moduleDefinition in await query.ExecuteNextAsync())
                {
                    moduleDefinitions.Add(moduleDefinition);
                }
            }

            return moduleDefinitions;
        }

        public async Task<int> Reset(JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<ModuleDefinition>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var moduleDefinitions = new List<ModuleDefinition>();

            while (documentQuery.HasMoreResults)
            {
                foreach (ModuleDefinition moduleDoc in await documentQuery.ExecuteNextAsync())
                {
                    moduleDefinitions.Add(moduleDoc);
                }
            }

            foreach (var moduleDefinition in moduleDefinitions)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: moduleDefinition.Id), new RequestOptions { PartitionKey = new PartitionKey(moduleDefinition.Id) });
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

        //public async Task<object> UpsertFromEstimate(JToken document) {
        //    JObject jToken = JTokenExtension.WalkNode(document);
        //    try {
        //        if (jToken.ContainsKey("moduleName")) {
        //            return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["moduleName"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["moduleName"].ToString()) });
        //        }

        //        return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);

            
        //    } catch(Exception ex) {
        //        Console.WriteLine(ex);
        //    }
        //    return null;
        //}
    }
}
