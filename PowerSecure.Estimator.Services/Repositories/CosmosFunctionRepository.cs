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
        private readonly IDictionary<string, IPrimitive> _primitives = Primitive.Load();

        public CosmosFunctionRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("functionsCollectionId", EnvironmentVariableTarget.Process);
        }

        public async Task<object> Upsert(JObject document)
        {
            IInstructionSet instructionSet = document.ToObject<InstructionSet>();
            DeleteFromCache(instructionSet);
            instructionSet = this.InsertNew(instructionSet.Module,
                instructionSet.Name,
                instructionSet.Instructions,
                instructionSet.StartDate,
                DateTime.Now,
                InstructionSet.Create,
                _primitives);

            if(document.ContainsKey("id"))
            {
                var id = document["id"];
                document = JObject.FromObject(instructionSet);
                document["id"] = id;
            }
            else
            {
                document = JObject.FromObject(instructionSet);
                document.Remove("id");
            }

            return await _Upsert(document);
        }

        public async Task<object> _Upsert(JObject document)
        {
            if (document.ContainsKey("id"))
            {
                return (Document)await _dbClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId: _databaseId, collectionId: _collectionId, documentId: document["id"].ToString()), document, new RequestOptions { PartitionKey = new PartitionKey(document["module"].ToString()) });
            }

            return (Document)await _dbClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), document);
        }

        public async Task<int> Delete(string id, IDictionary<string, string> queryParams)
        {
            DeleteFromCache(id);

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
                query = query.Where(f => f.Module == queryParams["module"]);
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

        private Dictionary<string, SortedSet<IInstructionSet>> InstructionSetCache { get; } = new Dictionary<string, SortedSet<IInstructionSet>>();
        
        private async Task<object> InitializeInstructionSetCache()
        {
            var query = _dbClient.CreateDocumentQuery<Function>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (Function function in await query.ExecuteNextAsync())
                {
                    InsertIntoCache(InstructionSet.FromFunction(function));
                }
            }

            return null;
        }

        void IInstructionSetRepository.Insert(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                return;
            }

            InsertIntoCache(instructionSet);
        }

        private void InsertIntoCache(IInstructionSet instructionSet)
        {
            if (!InstructionSetCache.ContainsKey(instructionSet.Key))
            {
                InstructionSetCache.Add(instructionSet.Key, new SortedSet<IInstructionSet>(Comparer<IInstructionSet>.Create((first, second) => first.CreationDate.CompareTo(second.CreationDate))));
            }

            InstructionSetCache[instructionSet.Key].Add(instructionSet);
        }

        void IInstructionSetRepository.Update(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                return;
            }

            var o = _Upsert(JObject.FromObject(instructionSet)).Result;

            InstructionSetCache[instructionSet.Key].Remove(instructionSet);
            InstructionSetCache[instructionSet.Key].Add(instructionSet);
        }

        private void DeleteFromCache(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                var o = InitializeInstructionSetCache().Result;
            }

            if(!InstructionSetCache.ContainsKey(instructionSet.Key))
            {
                return;
            }

            InstructionSetCache[instructionSet.Key].Remove(instructionSet);
            if (InstructionSetCache[instructionSet.Key].Count == 0)
            {
                InstructionSetCache.Remove(instructionSet.Key);
            }
        }

        private void DeleteFromCache(string id)
        {
            if (InstructionSetCache.Count == 0)
            {
                var o = InitializeInstructionSetCache().Result;
            }

            IInstructionSet deletedInstructionSet = InstructionSetCache.SelectMany(x => x.Value).FirstOrDefault(i => i.Id == id);

            if (deletedInstructionSet != null)
            {
                DeleteFromCache(deletedInstructionSet);

                InstructionSetCache.SelectMany(x => x.Value)
                            .Where(x => x.ChildInstructionSets.Contains(deletedInstructionSet.Key))
                            .ToList()
                            .ForEach(instructionSet => ((IInstructionSetRepository)this).Update(InstructionSet.Create(instructionSet.Id,
                                instructionSet.Module,
                                instructionSet.Name,
                                instructionSet.Instructions,
                                instructionSet.Parameters.Union(new List<string> { deletedInstructionSet.Key }),
                                instructionSet.ChildInstructionSets.Where(x => x != deletedInstructionSet.Key),
                                instructionSet.StartDate,
                                instructionSet.CreationDate)));
            }
        }

        bool IInstructionSetRepository.ContainsKey(string key)
        {
            if (InstructionSetCache.Count == 0)
            {
                var o = InitializeInstructionSetCache().Result;
            }

            return InstructionSetCache.ContainsKey(key);
        }

        IEnumerable<IInstructionSet> IInstructionSetRepository.SelectByKey(IEnumerable<string> instructionSetKeys, DateTime effectiveDate)
        {
            if (InstructionSetCache.Count == 0)
            {
                var o = InitializeInstructionSetCache().Result;
            }

            foreach (string instructionSetKey in instructionSetKeys)
            {
                if (InstructionSetCache.TryGetValue(instructionSetKey, out SortedSet<IInstructionSet> instructionSets))
                {
                    yield return instructionSets.Where(x => x.StartDate <= effectiveDate).OrderByDescending(x => x.CreationDate).First();
                }
            }
        }

        IEnumerable<IInstructionSet> IInstructionSetRepository.SelectByParameter(string parameter)
        {
            if (InstructionSetCache.Count == 0)
            {
                var o = InitializeInstructionSetCache().Result;
            }

            return InstructionSetCache.SelectMany(x => x.Value)
                        .Where(x => x.Parameters.Contains(parameter))
                        .ToList(); /* have to project to a new list to allow dictionary modification*/
        }
    }
}
