﻿using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class IInstructionSetMixin
    {
        public static object Evaluate(this IInstructionSet instructionSet, IDictionary<string, object> parameters, IDictionary<string, IFunction> functions, IReferenceDataRepository referenceDataRepository, IInstructionSetRepository instructionSetRepository, DateTime effectiveDate)
        {
            return new UnresolvedParameter() { Token = JObject.Parse(instructionSet.Instructions), Parameters = parameters, Functions = functions, ReferenceDataRepository = referenceDataRepository, InstructionSetRepository = instructionSetRepository, EffectiveDate = effectiveDate }.Resolve();
        }
    }
}
