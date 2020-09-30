using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosFunctionRepository : IFunctionRepository, IInstructionSetRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly IDictionary<string, IFunction> _functions = Primitive.Load();

        public CosmosFunctionRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = AppSettings.Get("databaseId");
            _collectionId = AppSettings.Get("functionsCollectionId");
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

            var query = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.Id == id)
                .AsDocumentQuery();

            var list = new List<Document>();
            while (query.HasMoreResults)
            {
                foreach (Function factor in await query.ExecuteNextAsync())
                {
                    list.Add(await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: factor.Id), new RequestOptions { PartitionKey = new PartitionKey(factor.Module) }));
                }
            }

            return list.Count;
        }

        public async Task<object> List(IDictionary<string, string> queryParams)
        {
            IQueryable<Function> query = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true });

            if (queryParams.ContainsKey("module"))
            {
                query = query.Where(f => f.Module == queryParams["module"].ToLower());
            }
            if (queryParams.ContainsKey("name"))
            {
                query = query.Where(f => f.Name == queryParams["name"].ToLower());
            }

            var functions = new List<Function>();

            var documentQuery = query.AsDocumentQuery();

            bool reportFullObject = (queryParams.TryGetValue("object", out string value) && value.ToLower() == "full");
            while (documentQuery.HasMoreResults)
            {
                foreach (Function function in await documentQuery.ExecuteNextAsync())
                {
                    if (!reportFullObject)
                    {
                        function.Rest = null;
                    }
                    functions.Add(function);
                }
            }

            return functions;
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            if (queryParams.ContainsKey("module"))
            {
                return (Document)await _dbClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(queryParams["module"]) });
            }

            var query = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.Id == id)
                .AsDocumentQuery();

            var functions = new List<Function>();

            while (query.HasMoreResults)
            {
                foreach (Function function in await query.ExecuteNextAsync())
                {
                    functions.Add(function);
                }
            }

            return functions;
        }

        IInstructionSet IInstructionSetRepository.Get(string module, string name, DateTime effectiveDate)
        {
            var query = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId))
                .Where(f => f.Module == module)
                .Where(f => f.Name == name)
                .AsDocumentQuery();

            var instructionSets = new List<IInstructionSet>();

            Task.Run(async () =>
            {
                while (query.HasMoreResults)
                {
                    foreach (InstructionSet instructionSet in await query.ExecuteNextAsync())
                    {
                        instructionSets.Add(instructionSet);
                    }
                }
            }).GetAwaiter().GetResult();

            return instructionSets.Where(x => x.StartDate <= effectiveDate).OrderByDescending(x => x.CreationDate).FirstOrDefault();
        }

        IInstructionSet IInstructionSetRepository.Get(string key, DateTime effectiveDate)
        {
            int moduleDelimiter = key.IndexOf(".");
            return ((IInstructionSetRepository)this).Get(key.Substring(0, moduleDelimiter), key.Substring(moduleDelimiter + 1), effectiveDate);
        }

        public async Task<int> Reset(string module, JToken jToken)
        {
            var documentQuery = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = false })
                .Where(f => f.Module == module).AsDocumentQuery();

            var ids = new List<string>();

            while (documentQuery.HasMoreResults)
            {
                foreach (Function function in await documentQuery.ExecuteNextAsync())
                {
                    ids.Add(function.Id);
                }
            }

            foreach (var id in ids)
            {
                await _dbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: id), new RequestOptions { PartitionKey = new PartitionKey(module) });
            }

            int count = 0;
            foreach (var child in jToken.Children())
            {
                if (child.Type != JTokenType.Object)
                {
                    continue;
                }

                var instructionSet = child.ToObject<InstructionSet>();

                await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), instructionSet);

                count++;
            }

            return count;
        }
    }
}
