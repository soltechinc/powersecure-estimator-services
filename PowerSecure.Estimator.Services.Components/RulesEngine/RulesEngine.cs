using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Conversions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine {
    public class RulesEngine
    {
        public IDictionary<string, object> EvaluateDataSheet(IDictionary<string, object> dataSheet, DateTime effectiveDate, IDictionary<string, IFunction> functions, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log)
        {
            var suppliedParameters = new HashSet<string>();
            var missingParameters = new HashSet<string>();
            var parameters = new Dictionary<string, object>();
            
            foreach (var parameter in dataSheet)
            {
                if(parameter.Value == null)
                {
                    missingParameters.Add(parameter.Key.Trim().ToLower());
                }
                else
                {
                    suppliedParameters.Add(parameter.Key.Trim().ToLower());
                    parameters.Add(parameter.Key?.Trim()?.ToLower(), parameter.Value);
                }
            }
            log.LogInformation("Parameters first pass: " + JToken.FromObject(dataSheet));

            foreach (var key in missingParameters)
            {
                if(!parameters.ContainsKey(key))
                {
                    log.LogInformation($"Adding parameter {key}");
                    parameters.Add(key, instructionSetRepository.Get(key, effectiveDate)?.Evaluate(parameters, functions, referenceDataRepository, instructionSetRepository, effectiveDate, log));
                }

                dataSheet[key] = parameters[key]?.ToRawString();
            }

            return dataSheet;
        }
    }
}
