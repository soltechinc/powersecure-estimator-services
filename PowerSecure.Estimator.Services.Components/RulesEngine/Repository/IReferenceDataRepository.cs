using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public interface IReferenceDataRepository
    {
        object Lookup(string dataSetName, (string SearchParam, string Value)[] criteria, DateTime effectiveDate, string returnFieldName);
    }
}
