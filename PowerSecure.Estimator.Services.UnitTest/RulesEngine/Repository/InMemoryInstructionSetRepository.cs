using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository
{
    public class InMemoryInstructionSetRepository : IInstructionSetRepository
    {
        public Dictionary<string, SortedSet<IInstructionSet>> Items { get; } = new Dictionary<string, SortedSet<IInstructionSet>>();

        public void Insert(IInstructionSet instructionSet)
        {
            if (!Items.ContainsKey(instructionSet.Key))
            {
                Items.Add(instructionSet.Key, new SortedSet<IInstructionSet>(Comparer<IInstructionSet>.Create((first, second) => first.CreationDate.CompareTo(second.CreationDate))));
            }

            Items[instructionSet.Key].Add(instructionSet);
        }

        public void Update(IInstructionSet instructionSet)
        {
            Items[instructionSet.Key].Remove(instructionSet);
            Items[instructionSet.Key].Add(instructionSet);
        }

        public bool ContainsKey(string key)
        {
            return Items.ContainsKey(key);
        }

        public IEnumerable<IInstructionSet> SelectByKey(IEnumerable<string> instructionSetKeys, DateTime effectiveDate)
        {
            foreach (string instructionSetKey in instructionSetKeys)
            {
                if (Items.TryGetValue(instructionSetKey, out SortedSet<IInstructionSet> instructionSets))
                {
                    yield return instructionSets.Where(x => x.StartDate <= effectiveDate).OrderByDescending(x => x.CreationDate).First();
                }
                else
                {
                    //handle error condition
                }
            }
        }

        public IEnumerable<IInstructionSet> SelectByParameter(string parameter)
        {
            return Items.SelectMany(x => x.Value)
                        .Where(x => x.Parameters.Contains(parameter))
                        .ToList(); /* have to project to a new list to allow dictionary modification*/
        }
        
        public void Load(string csvFilename, IDictionary<string, IPrimitive> primitives)
        {
            //get csv data
            using (var stream = new FileStream(csvFilename, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                string[] headerRow = csvReader.Context.HeaderRecord;

                while (csvReader.Read())
                {
                    this.InsertNew(csvReader.GetField("Module"),csvReader.GetField("Name"), csvReader.GetField("InstructionSet"), csvReader.GetField<DateTime>("StartDate"), csvReader.GetField<DateTime>("CreationDate"), InstructionSet.Create, primitives);
                }
            }
        }
    }
}
