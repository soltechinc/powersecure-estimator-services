using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.UnitTest.RulesEngine.Repository
{
    public class InMemoryReferenceDataRepository : IReferenceDataRepository
    {
        public object Lookup(string dataSetName, (string SearchParam, string Value)[] criteria, string returnFieldName)
        {
            return 0m;
        }
    }
}
