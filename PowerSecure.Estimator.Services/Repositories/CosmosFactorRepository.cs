using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosFactorRepository : IFactorRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosFactorRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("factorsCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document)
        {
            if(document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["key"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("key"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(queryParams["key"]) });
                return 1;
            }

            var query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.Id == id)
                .AsDocumentQuery();

            var list = new List<ResourceResponse<Document>>();
            while (query.HasMoreResults)
            {
                foreach (Factor factor in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: factor.Id), new RequestOptions { PartitionKey = new PartitionKey(factor.Key) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string,string> queryParams)
        {
            IQueryable<Factor> query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true });

            if(queryParams.ContainsKey("module"))
            {
                query = query.Where(f => f.Module == queryParams["module"]);
            }

            var factors = new List<Factor>();

            var documentQuery = query.AsDocumentQuery();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value))
            {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
            while (documentQuery.HasMoreResults)
            {
                foreach (Factor factor in await documentQuery.ExecuteNextAsync())
                {
                    if (!reportFullObject)
                    {
                        factor.Rest = null;
                    }
                    factors.Add(factor);
                }
            }

            return factors;
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("key"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(queryParams["key"]) });
            }
            
            var query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.Id == id)
                .AsDocumentQuery();

            var factors = new List<Factor>();

            while (query.HasMoreResults)
            {
                foreach (Factor factor in await query.ExecuteNextAsync())
                {
                    factors.Add(factor);
                }
            }

            return factors;
        }
    }
}
