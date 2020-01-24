﻿using System;
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
        public Dictionary<string, IInstructionSet> Items { get; } = new Dictionary<string, IInstructionSet>();

        public void Insert(IInstructionSet instructionSet)
        {
            if (Items.ContainsKey(instructionSet.Key))
            {
                throw new Exception();
            }

            Items.Add(instructionSet.Key, instructionSet);
        }
    
        public void Update(IInstructionSet instructionSet)
        {
            if(!Items.ContainsKey(instructionSet.Key))
            {
                throw new Exception();
            }

            Items[instructionSet.Key] = instructionSet;
        }

        public bool ContainsKey(string key)
        {
            return Items.ContainsKey(key);
        }

        public IEnumerable<IInstructionSet> SelectByKey(params string[] instructionSetKeys)
        {
            return SelectByKey(instructionSetKeys.AsEnumerable());
        }

        public IEnumerable<IInstructionSet> SelectByKey(IEnumerable<string> instructionSetKeyss)
        {
            foreach (string instructionSetKey in instructionSetKeyss)
            {
                if (Items.TryGetValue(instructionSetKey, out IInstructionSet instructionSet))
                {
                    yield return instructionSet;
                }
                else
                {
                    //handle error condition
                }
            }
        }

        public IEnumerable<IInstructionSet> SelectByParameter(string parameter)
        {
            return Items.Select(x => x.Value)
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
                    this.InsertNew(csvReader.GetField("Module"),csvReader.GetField("Name"), csvReader.GetField("InstructionSet"), InstructionSet.Create, primitives);
                }
            }
        }
    }
}
