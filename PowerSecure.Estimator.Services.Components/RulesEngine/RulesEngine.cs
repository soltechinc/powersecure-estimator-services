﻿using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
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

            foreach (var key in missingParameters)
            {
                if(!parameters.ContainsKey(key))
                {
                    IInstructionSet instructionSet = instructionSetRepository.Get(key, effectiveDate);
                    if(instructionSet == null)
                    {
                        log.LogWarning($"Unable to find instruction set {key}");
                    }

                    parameters.Add(key, instructionSet?.Evaluate(parameters, functions, referenceDataRepository, instructionSetRepository, effectiveDate, log));
                }

                dataSheet[key] = parameters[key];
            }

            return dataSheet;
        }
    }
}
