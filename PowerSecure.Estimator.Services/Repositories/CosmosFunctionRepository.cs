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
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine;

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
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("functionsCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
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

            var functions = new List<Function>();

            var documentQuery = query.AsDocumentQuery();

            bool reportFullObject = false;
            if (queryParams.TryGetValue("object", out string value))
            {
                reportFullObject = (value.Trim().ToLower() == "full");
            }
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
            var x = key.Split('.');
            return ((IInstructionSetRepository)this).Get(x[0], x[1], effectiveDate);
        }
    }
}
