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
        public Dictionary<string, List<IInstructionSet>> Items { get; } = new Dictionary<string, List<IInstructionSet>>();

        public void Insert(IInstructionSet instructionSet)
        {
            if (!Items.ContainsKey(((TestInstructionSet)instructionSet).Key))
            {
                Items.Add(((TestInstructionSet)instructionSet).Key, new List<IInstructionSet>());
            }
            Items[((TestInstructionSet)instructionSet).Key].Add(instructionSet);
        }

        public IInstructionSet Get(string module, string name, DateTime effectiveDate)
        {            
            return Get($"{module}.{name}",effectiveDate);
        }

        public IInstructionSet Get(string key, DateTime effectiveDate)
        {
            if (Items.TryGetValue(key.ToLower(), out List<IInstructionSet> instructionSets))
            {
                return instructionSets.Where(x => x.StartDate <= effectiveDate).OrderByDescending(x => x.CreationDate).First();
            }

            return null;
        }
        
        public void Load(string csvFilename, IDictionary<string, IFunction> primitives)
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
                    string key = string.Format("{0}.{1}", csvReader.GetField("Module"), csvReader.GetField("Name")).ToLower();
                    if(!Items.ContainsKey(key))
                    {
                        Items.Add(key, new List<IInstructionSet>());
                    }
                    Items[key].Add(this.ValidateInstructionSet(csvReader.GetField("Module"),csvReader.GetField("Name"), csvReader.GetField("InstructionSet"), csvReader.GetField<DateTime>("StartDate"), csvReader.GetField<DateTime>("CreationDate"), TestInstructionSet.Create, primitives));
                }
            }
        }
    }
}
