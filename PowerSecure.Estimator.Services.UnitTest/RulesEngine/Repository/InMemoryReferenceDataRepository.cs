using CsvHelper;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository
{
    public class InMemoryReferenceDataRepository : IReferenceDataRepository
    {
        public List<Dictionary<string, string>> Items { get; } = new List<Dictionary<string, string>>();

        public object Lookup(string dataSetName, (string SearchParam, string Value)[] criteria, DateTime effectiveDate, string returnFieldName)
        {
            var matches = Items.Where(dict => criteria.All(p => dict.ContainsKey(p.SearchParam.ToLower()) && dict[p.SearchParam.ToLower()] == p.Value.ToLower()) && dict["returnattribute"] == returnFieldName.ToLower()).ToArray();

            if (matches.Length == 0)
            {
                throw new Exception("not found");
            }

            return matches.Where(dict => DateTime.Parse(dict["startdate"]) < effectiveDate)
                          .OrderByDescending(dict => DateTime.Parse(dict["creationdate"]))
                          .First()["returnvalue"].ToLower();
        }

        public void Load(string csvFilename)
        {
            //get csv data
            using (var stream = new FileStream(csvFilename, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                string[] headerRow = csvReader.Context.HeaderRecord;

                while(csvReader.Read())
                {
                    var dict = new Dictionary<string, string>();
                    foreach (string column in headerRow)
                    {
                        if (string.IsNullOrEmpty(column))
                            continue;
                        string value = csvReader.GetField(column);
                        if (string.IsNullOrEmpty(value))
                            continue;
                        dict.Add(column.ToLower().Trim(), value.ToLower().Trim());
                    }
                    Items.Add(dict);
                }
            }
        }
    }
}
