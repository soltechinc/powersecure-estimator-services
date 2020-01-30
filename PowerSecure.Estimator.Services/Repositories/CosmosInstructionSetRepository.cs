using Microsoft.Azure.Documents.Client;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents.Linq;

namespace PowerSecure.Estimator.Services.Repositories
{
    public class CosmosInstructionSetRepository : IInstructionSetRepository
    {
        private readonly DocumentClient _dbClient;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public CosmosInstructionSetRepository(DocumentClient dbClient)
        {
            _dbClient = dbClient;
            _databaseId = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            _collectionId = Environment.GetEnvironmentVariable("functionsCollectionId", EnvironmentVariableTarget.Process);
        }

        private Dictionary<string, SortedSet<IInstructionSet>> InstructionSetCache { get; } = new Dictionary<string, SortedSet<IInstructionSet>>();

        private async void InitializeInstructionSetCache()
        {
            var query = _dbClient.CreateDocumentQuery<InstructionSet>(UriFactory.CreateDocumentCollectionUri(databaseId: _databaseId, collectionId: _collectionId), new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();
            
            while (query.HasMoreResults)
            {
                foreach (InstructionSet instructionSet in await query.ExecuteNextAsync())
                {
                    _Insert(instructionSet);
                }
            }
        }

        public void Insert(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                return;
            }

            _Insert(instructionSet);
        }

        private void _Insert(IInstructionSet instructionSet)
        {
            if (!InstructionSetCache.ContainsKey(instructionSet.Key))
            {
                InstructionSetCache.Add(instructionSet.Key, new SortedSet<IInstructionSet>(Comparer<IInstructionSet>.Create((first, second) => first.CreationDate.CompareTo(second.CreationDate))));
            }

            InstructionSetCache[instructionSet.Key].Add(instructionSet);
        }

        public void Update(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                return;
            }

            InstructionSetCache[instructionSet.Key].Remove(instructionSet);
            InstructionSetCache[instructionSet.Key].Add(instructionSet);
        }

        public void Delete(IInstructionSet instructionSet)
        {
            if (InstructionSetCache.Count == 0)
            {
                return;
            }

            InstructionSetCache[instructionSet.Key].Remove(instructionSet);
            if(InstructionSetCache[instructionSet.Key].Count == 0)
            {
                InstructionSetCache.Remove(instructionSet.Key);
            }
        }

        public bool ContainsKey(string key)
        {
            if(InstructionSetCache.Count == 0)
            {
                InitializeInstructionSetCache();
            }

            return InstructionSetCache.ContainsKey(key);
        }

        public IEnumerable<IInstructionSet> SelectByKey(IEnumerable<string> instructionSetKeys, DateTime effectiveDate)
        {
            if (InstructionSetCache.Count == 0)
            {
                InitializeInstructionSetCache();
            }

            foreach (string instructionSetKey in instructionSetKeys)
            {
                if (InstructionSetCache.TryGetValue(instructionSetKey, out SortedSet<IInstructionSet> instructionSets))
                {
                    yield return instructionSets.Where(x => x.StartDate <= effectiveDate).OrderByDescending(x => x.CreationDate).First();
                }
            }
        }

        public IEnumerable<IInstructionSet> SelectByParameter(string parameter)
        {
            if (InstructionSetCache.Count == 0)
            {
                InitializeInstructionSetCache();
            }

            return InstructionSetCache.SelectMany(x => x.Value)
                        .Where(x => x.Parameters.Contains(parameter))
                        .ToList(); /* have to project to a new list to allow dictionary modification*/
        }
    }
}
