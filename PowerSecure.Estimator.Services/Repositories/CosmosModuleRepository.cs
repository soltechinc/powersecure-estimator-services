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
            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(m => m.ModuleTitle == document["moduleTitle"].ToString())
                .AsDocumentQuery();

            var list = new List<ResourceResponse<Document>>();
            while (query.HasMoreResults)
            {
                foreach (Module m in await query.ExecuteNextAsync())
                {
                    document["id"] = m.Id;
                    list.Add(await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: m.Id), document));
                }
            }

            if (list.Count == 0)
            {
                return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
            }
            else
            {
                return list.Select(r => r.Resource);
            }
        }

        public async Task<object> Delete(string id)
        {
            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(m => m.ModuleTitle == id)
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

            var documentIds = new List<string>();

            while (query.HasMoreResults)
            {
                foreach (Module m in await query.ExecuteNextAsync())
                {
                    documentIds.Add(m.ModuleTitle);
                }
            }

            return documentIds;
        }

        public async Task<object> Get(string id)
        {
            var query = _dbClient.CreateDocumentQuery<Module>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(m => m.ModuleTitle == id)
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
