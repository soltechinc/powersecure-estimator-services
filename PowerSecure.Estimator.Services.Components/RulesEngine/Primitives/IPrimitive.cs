﻿using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
    public interface IPrimitive
    {
        string Name { get; }

        bool ResolveParameters { get; }

        decimal Invoke(object[] parameters, IReferenceDataRepository referenceDataRepository);

        (bool success, string message) Validate(JToken jToken);
    }
}
