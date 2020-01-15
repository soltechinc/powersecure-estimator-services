using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository
{
    public class InMemoryReferenceDataRepository : IReferenceDataRepository
    {
        public decimal Lookup(string dataSetName, KeyValuePair<string,string>[] criteria, string returnFieldName)
        {
            return 0;
        }
    }
}
