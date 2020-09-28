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
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System.Threading;
using System.Security.Cryptography;

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

        private static string CreateHash(string valueKey) {
            return MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(valueKey))
                      .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
        }

        private object CreateKey(JObject document) { 
        document["key"] = string.Join('-', string.Empty, document["module"], document["returnattribute"]);
        document["hash"] = CreateHash(document.Properties()
                                .Where(o => o.Name != "id" && o.Name != "hash" && !o.Name.StartsWith("_"))
                                .SelectMany(o => new string[] { o.Name, o.Value.ToString()
                                })
                                .OrderBy(s => s)
                                .Aggregate(new StringBuilder(), (sb, s) => sb.AppendFormat("-{0}", s)).ToString());
            if (!document.ContainsKey("creationdate")) {
                document.Add("creationdate", JToken.FromObject(DateTime.Now.ToString("M/d/yyyy")));
            }
            return document;
        }

        public async Task<object> UpsertList(JObject document) {
            JArray items = (JArray)document["items"];
            int length = items.Count;
            WaitHandle[] waitHandles = new WaitHandle[length];
            
            for (int i = 0; length > i; i++) {
                var j = i;
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                    Thread.Sleep(j * 1000);
                    Console.WriteLine("Thread{0} exits", j);
                    handle.Set();
                });
                waitHandles[j] = handle;
                thread.Start();
                CreateKey((JObject)items[i]);
                await Upsert((JObject)items[i]);
            }
            WaitHandle.WaitAll(waitHandles);
            return document;
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

        public async Task<object> List(IDictionary<string,string> queryParams)
        {
            IQueryable<Factor> query = _dbClient.CreateDocumentQuery<Factor>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true });

            if(queryParams.ContainsKey("module"))
            {
                query = query.Where(f => f.Module == queryParams["module"]);
            }
            if(queryParams.ContainsKey("returnattribute"))
            {
                query = query.Where(f => f.ReturnAttribute == queryParams["returnattribute"]);
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
            if(!queryParams.TryGetValue("key", out string value) || string.IsNullOrEmpty(value))
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

            Factor result = (await query.ExecuteNextAsync()).Where(f => DateTime.Parse(f.startdate.ToString()) < effectiveDate)
                          .OrderByDescending(f => f.creationdate.ToString())
                          .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return result.ReturnValue;
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

            int count = 0;
            foreach(var child in jToken.Children())
            {
                if(child.Type != JTokenType.Object)
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
