using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Repository
{
    public interface IReferenceDataRepository
    {
        decimal Lookup(string dataSetName, string[] criteria, string returnFieldName);
    }
}
