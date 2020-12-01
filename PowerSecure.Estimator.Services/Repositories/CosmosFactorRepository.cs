using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosFactorRepository : IFactorRepository, IReferenceDataRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosFactorRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseId");
            _collectionId = AppSettings.Get("factorsCollectionId");
        }
        
        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id") && document["id"] != null && !string.IsNullOrEmpty(document["id"].ToString()))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["module"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("module"))
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(queryParams["module"]) });
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
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: factor.Id), new RequestOptions { PartitionKey = new PartitionKey(factor.Module) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            IQueryable<Factor> query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true });

            if (queryParams.ContainsKey("module"))
            {
                query = query.Where(f => f.Module == queryParams["module"]);
            }
            if (queryParams.ContainsKey("returnattribute"))
            {
                query = query.Where(f => f.ReturnAttribute == queryParams["returnattribute"]);
            }

            var factors = new List<Factor>();

            var documentQuery = query.AsDocumentQuery();

            bool reportFullObject = (queryParams.TryGetValue("object", out string value) && value.ToLower() == "full");
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
            if (queryParams.ContainsKey("module"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(queryParams["module"]) });
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

        public async Task<object> Lookup(IDictionary<string, string> queryParams)
        {
            if (!queryParams.TryGetValue("key", out string value) || string.IsNullOrEmpty(value))
            {
                return null;
            }

            return await AsyncLookup(string.Empty, new (string, string)[] { ("module", "all") }, DateTime.Now, queryParams["key"]);
        }

        object IReferenceDataRepository.Lookup(string dataSetName, (string SearchParam, string Value)[] criteria, DateTime effectiveDate, string returnFieldName)
        {
            return AsyncLookup(dataSetName, criteria, effectiveDate, returnFieldName).GetAwaiter().GetResult();
        }

        private async Task<object> AsyncLookup(string dataSetName, (string SearchParam, string Value)[] criteria, DateTime effectiveDate, string returnFieldName)
        {
            var str = new StringBuilder("select * from f where f.returnattribute = \"" + returnFieldName.ToLower() + "\"");
            foreach ((string searchParam, string value) in criteria)
            {
                if (searchParam == null || value == null)
                {
                    return null;
                }
                str.Append(" and f[\"" + searchParam.ToLower().Trim() + "\"] = \"" + value.ToLower().Trim() + "\"");
            }

            var query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId),
                            str.ToString()).AsDocumentQuery();

            Factor result = (await query.ExecuteNextAsync()).Where(f => DateTime.Parse(f.startdate.ToString()) <= effectiveDate)
                          .OrderByDescending(f => DateTime.Parse(f.creationdate.ToString()))
                          .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return result.ReturnValue;
        }

        public async Task<int> Import(JToken jToken)
        {
            int count = 0;
            foreach (var child in jToken.Children())
            {
                if (child.Type != JTokenType.Object)
                {
                    continue;
                }

                var document = (JObject)child;

                {
                    var doc = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = false })
                        .Where(x => x.Module == document["module"].ToString())
                        .Where(x => x.Id == document["id"].ToString()).AsEnumerable().FirstOrDefault();

                    if (doc != null)
                    {
                        await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["module"].ToString()) });
                    }
                    else
                    {
                        await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
                    }
                }

                count++;
            }

            return count;
        }

        public async Task<int> Reset(string module, JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = false })
                .Where(f => f.Module == module).AsDocumentQuery();

            var ids = new List<string>();

            while (documentQuery.HasMoreResults)
            {
                foreach (Factor factor in await documentQuery.ExecuteNextAsync())
                {
                    ids.Add(factor.Id);
                }
            }

            foreach (var id in ids)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(module) });
            }

            return await Import(jToken);
        }
    }
}
