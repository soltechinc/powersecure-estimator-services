﻿using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using PowerSecure.Estimator.Services.Endpoints;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using PowerSecure.Estimator.Services.Models;
using Microsoft.Azure.Documents;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IDictionary<string, IFunction> _functions;
        private readonly ILogger _log;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IBusinessOpportunityLineItemRepository _businessOpportunityLineItemRepository;

        public EstimateService(IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, IEstimateRepository estimateRepository, IBusinessOpportunityLineItemRepository businessOpportunityLineItemRepository, ILogger log)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _estimateRepository = estimateRepository;
            _businessOpportunityLineItemRepository = businessOpportunityLineItemRepository;
            _functions = Primitive.Load();
            _log = log;
        }

        public EstimateService(IEstimateRepository estimateRepository) {
            _estimateRepository = estimateRepository;
        }

        public async Task<(object, string)> Evaluate(JObject uiInputs)
        {
            var dataSheet = new Dictionary<string, object>();
            bool hasModuleTitle = uiInputs.Properties().Any(prop => prop.Name == "moduleTitle");

            string moduleName = !hasModuleTitle ? string.Empty : uiInputs.Properties().Where(prop => prop.Name == "moduleTitle").First().Value.ToObject<string>().ToLower().Trim();

            if (!hasModuleTitle || uiInputs.Properties().Any(prop => prop.Name.ToLower() == "datasheet"))
            {
                dataSheet = (Dictionary<string,object>)DataSheetFromJToken(uiInputs);
                IncludeEstimateData(uiInputs, dataSheet, moduleName);
            }
            else
            {
                IncludeEstimateData(uiInputs, dataSheet, moduleName);

                //Translate into data sheet
                ParseFromJson(uiInputs, dataSheet, moduleName);
            }

            if(!dataSheet.ContainsKey("all.effectivedate"))
            {
                dataSheet.Add("all.effectivedate", DateTime.Now.ToString("M/d/yyyy"));
            }
            
            new RulesEngine().EvaluateDataSheet(dataSheet, DateTime.Now, _functions, _instructionSetRepository, _referenceDataRepository, _log);

            if (!hasModuleTitle || uiInputs.Properties().Any(prop => prop.Name.ToLower() == "datasheet"))
            {
                return (JObject.FromObject(dataSheet), "");
            }
            else
            {
                //Convert back
                ParseToJson(uiInputs, dataSheet, moduleName);
            }

            return (uiInputs, "");
        }

        private object DataSheetFromJToken(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Array:
                    {
                        var list = new List<Dictionary<string, object>>();
                        foreach(var element in (JArray)jToken)
                        {
                            list.Add((Dictionary<string, object>)DataSheetFromJToken(element));
                        }
                        return list;
                    }
                case JTokenType.Object:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in ((JObject)jToken).Properties())
                        {
                            dict.Add(prop.Name.ToLower(), DataSheetFromJToken(prop.Value));
                        }
                        return dict;
                    }
                case JTokenType.Integer:
                    return Convert.ToDecimal(jToken.ToObject<int>());
                case JTokenType.Float:
                    return Convert.ToDecimal(jToken.ToObject<float>());
                case JTokenType.Boolean:
                    return jToken.ToObject<bool>();
                case JTokenType.Null:
                    return null;
                default:
                    return jToken.ToString();
            }
        }

        private void IncludeEstimateData(JObject uiInputs, Dictionary<string, object> dataSheet, string moduleName)
        {
            string estimateId = uiInputs.Properties().Where(prop => prop.Name.ToLower() == "estimateid").FirstOrDefault()?.Value?.ToObject<string>()?.Trim();
            string boliNumber = uiInputs.Properties().Where(prop => prop.Name.ToLower() == "bolinumber").FirstOrDefault()?.Value?.ToObject<string>()?.Trim();
            string boliId = uiInputs.Properties().Where(prop => prop.Name.ToLower() == "boliid").FirstOrDefault()?.Value?.ToObject<string>()?.Trim();

            if (estimateId != null && boliNumber != null && boliId != null)
            {
                {
                    Estimate estimate = JObject.Parse(((Document)_estimateRepository.Get(boliNumber, new Dictionary<string, string> { ["id"] = estimateId }).GetAwaiter().GetResult()).ToString()).ToObject<Estimate>();
                    if (!string.IsNullOrEmpty(estimate.ProjectType))
                    {
                        dataSheet.Add("all.projecttype", $"${estimate.ProjectType}");
                    }
                    if (!string.IsNullOrEmpty(estimate.OutsideEquipmentPercent))
                    {
                        dataSheet.Add("all.outsideequipmentpercentage", (decimal.Parse(estimate.OutsideEquipmentPercent) / 100m));
                    }
                    if (!string.IsNullOrEmpty(estimate.DesiredRateForInstall))
                    {
                        dataSheet.Add("all.desiredinstallrate", decimal.Parse(estimate.DesiredRateForInstall));
                    }

                    foreach (var module in estimate.Modules ?? Enumerable.Empty<ModuleDefinition>())
                    {
                        string moduleTitle = module.ModuleTitle.ToLower();

                        if (moduleTitle == moduleName)
                        {
                            continue;
                        }

                        if (!dataSheet.ContainsKey(moduleTitle))
                        {
                            dataSheet.Add(moduleTitle, new List<Dictionary<string, object>>());
                        }

                        var moduleDataSheet = new Dictionary<string, object>();
                        ParseFromJson(JObject.FromObject(module), moduleDataSheet, moduleTitle);
                        ((List<Dictionary<string, object>>)dataSheet[moduleTitle]).Add(moduleDataSheet);
                    }
                }
                {
                    BusinessOpportunityLineItem boli = JObject.Parse(((Document)_businessOpportunityLineItemRepository.Get(boliNumber, new Dictionary<string, string> { ["id"] = boliId }).GetAwaiter().GetResult()).ToString()).ToObject<BusinessOpportunityLineItem>();
                    if (!string.IsNullOrEmpty(boli.State))
                    {
                        dataSheet.Add("all.usstate", $"${boli.State.ToLower()}");
                    }
                    if(!string.IsNullOrEmpty(boli.City))
                    {
                        dataSheet.Add("all.uscity", $"${boli.City.ToLower()}");
                    }
                }
            }
        }

        private void ParseFromJson(JObject uiInputs, Dictionary<string,object> dataSheet, string moduleName)
        {
            string submoduleName = string.Empty;
            string fullSubmoduleName = string.Empty;

            var submoduleList = new List<(string, int)>();
            var submoduleCount = new Dictionary<string, int>();
            uiInputs.WalkNodes(PreOrder: jToken =>
                {
                    if (jToken.Path.Contains("moduleInputs") && jToken.Path.Contains("submoduleData"))
                    {
                        return;
                    }

                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                if (jObject.Properties().Any(prop => prop.Name == "inputType") && jObject["inputType"].ToObject<string>().ToLower() == "file input")
                                {
                                    break;
                                }

                                if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    (jObject.Properties().Any(prop => prop.Name == "inputValue") ||
                                    jObject.Properties().Any(prop => prop.Name == "quantity")))
                                {

                                    bool isCalculated = IsCalculated(jObject);

                                    string name = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                    if (name.StartsWith("**customitem**"))
                                    {
                                        string customItemKey;
                                        
                                        if (jToken.Path.Contains("submoduleData"))
                                        {
                                            customItemKey = $"{moduleName}.{submoduleName}customitem";
                                        }
                                        else
                                        {
                                            customItemKey = $"{moduleName}.customitem";
                                        }

                                        if (!dataSheet.ContainsKey(customItemKey))
                                        {
                                            dataSheet.Add(customItemKey, new List<Dictionary<string, object>>());
                                        }

                                        var dict = new Dictionary<string, object>();
                                        foreach (var prop in jObject.Properties())
                                        {
                                            if(prop.Name == "variableName")
                                            {
                                                continue;
                                            }

                                            object inputValue;
                                            JToken inputValueFromJson = prop.Value;
                                            switch (inputValueFromJson.Type)
                                            {
                                                case JTokenType.Integer:
                                                    {
                                                        decimal? value = Convert.ToDecimal(inputValueFromJson.ToObject<int>());
                                                        inputValue = value;
                                                        break;
                                                    }
                                                case JTokenType.Float:
                                                    {
                                                        decimal? value = Convert.ToDecimal(inputValueFromJson.ToObject<float>());
                                                        inputValue = value;
                                                        break;
                                                    }
                                                case JTokenType.Boolean:
                                                    {
                                                        bool? value = inputValueFromJson.ToObject<bool>();
                                                        inputValue = value;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        string value = inputValueFromJson.ToObject<string>();
                                                        inputValue = string.IsNullOrWhiteSpace(value) ? null : value;
                                                        break;
                                                    }
                                            }

                                            dict.Add($"{customItemKey}.{prop.Name.ToLower()}", inputValue);
                                        }

                                        ((List<Dictionary<string, object>>)dataSheet[customItemKey]).Add(dict);
                                    }
                                    else
                                    {
                                        object inputValue;
                                        JToken inputValueFromJson = jObject.Properties().Any(prop => prop.Name == "inputValue") ? jObject["inputValue"] : jObject["quantity"];
                                        if (inputValueFromJson.Type == JTokenType.Object)
                                        {
                                            inputValueFromJson = ((JObject)inputValueFromJson)["value"];
                                        }
                                        switch (inputValueFromJson.Type)
                                        {
                                            case JTokenType.Integer:
                                                {
                                                    decimal? value = Convert.ToDecimal(inputValueFromJson.ToObject<int>());
                                                    inputValue = value;
                                                    break;
                                                }
                                            case JTokenType.Float:
                                                {
                                                    decimal? value = Convert.ToDecimal(inputValueFromJson.ToObject<float>());
                                                    inputValue = value;
                                                    break;
                                                }
                                            case JTokenType.Boolean:
                                                {
                                                    bool? value = inputValueFromJson.ToObject<bool>();
                                                    inputValue = value;
                                                    break;
                                                }
                                            default:
                                                {
                                                    string value = inputValueFromJson.ToObject<string>();
                                                    inputValue = string.IsNullOrWhiteSpace(value) ? null : value;
                                                    break;
                                                }
                                        }

                                        if (jToken.Path.Contains("submoduleData"))
                                        {
                                            List<Dictionary<string, object>> dataSheetList = (List<Dictionary<string, object>>)dataSheet[fullSubmoduleName];

                                            if (!name.Contains("."))
                                            {
                                                name = $"{moduleName}.{submoduleName}.{name}";
                                            }
                                            var lastDataSheet = dataSheetList[dataSheetList.Count - 1];
                                            if (lastDataSheet.ContainsKey(name))
                                            {
                                                lastDataSheet = new Dictionary<string, object>();
                                                dataSheetList.Add(lastDataSheet);
                                                if (jToken.Path.Contains("currentSubmodule"))
                                                {
                                                    lastDataSheet.Add("currentsubmodule", true);
                                                }
                                                else
                                                {
                                                    lastDataSheet.Add("currentsubmodule", false);
                                                }
                                            }

                                            if (!lastDataSheet.ContainsKey(name))
                                            {
                                                lastDataSheet.Add(name, isCalculated ? null : inputValue);
                                            }
                                            else if (lastDataSheet[name] == null)
                                            {
                                                lastDataSheet[name] = isCalculated ? null : inputValue;
                                            }
                                        }
                                        else
                                        {
                                            if (!name.Contains("."))
                                            {
                                                name = $"{moduleName}.{name}";
                                            }
                                            if (!dataSheet.ContainsKey(name))
                                            {
                                                dataSheet.Add(name, isCalculated ? null : inputValue);
                                            }
                                            else if (dataSheet[name] == null)
                                            {
                                                dataSheet[name] = isCalculated ? null : inputValue;
                                            }
                                        }

                                        if (jObject.Properties().Any(prop => prop.Name == "quantity"))
                                        {
                                            string originalVariableName = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                            foreach (var prop in jObject.Properties())
                                            {
                                                if (prop.Name == "quantity")
                                                {
                                                    continue;
                                                }

                                                if (prop.Value.Type != JTokenType.String || !string.IsNullOrEmpty(prop.Value.ToObject<string>()))
                                                {
                                                    continue;
                                                }

                                                name = $"{originalVariableName}{prop.Name.ToLower().Trim()}";

                                                if (jToken.Path.Contains("submoduleData"))
                                                {
                                                    List<Dictionary<string, object>> dataSheetList = (List<Dictionary<string, object>>)dataSheet[fullSubmoduleName];

                                                    if (!name.Contains("."))
                                                    {
                                                        name = $"{moduleName}.{submoduleName}.{name}";
                                                    }
                                                    var lastDataSheet = dataSheetList[dataSheetList.Count - 1];
                                                    if (lastDataSheet.ContainsKey(name))
                                                    {
                                                        lastDataSheet = new Dictionary<string, object>();
                                                        dataSheetList.Add(lastDataSheet);
                                                        if (jToken.Path.Contains("currentSubmodule"))
                                                        {
                                                            lastDataSheet.Add("currentsubmodule", true);
                                                        }
                                                        else
                                                        {
                                                            lastDataSheet.Add("currentsubmodule", false);
                                                        }
                                                    }

                                                    if (!lastDataSheet.ContainsKey(name))
                                                    {
                                                        lastDataSheet.Add(name, null);
                                                    }
                                                    else if (lastDataSheet[name] == null)
                                                    {
                                                        lastDataSheet[name] = null;
                                                    }
                                                }
                                                else
                                                {
                                                    if (!name.Contains("."))
                                                    {
                                                        name = $"{moduleName}.{name}";
                                                    }
                                                    if (!dataSheet.ContainsKey(name))
                                                    {
                                                        dataSheet.Add(name, null);
                                                    }
                                                    else if (dataSheet[name] == null)
                                                    {
                                                        dataSheet[name] = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (jObject.Properties().Any(prop => prop.Name == "text") &&
                                    jObject.Properties().Any(prop => prop.Name == "align") ||
                                    jObject.Properties().Any(prop => prop.Name == "value"))
                                {
                                    if (jToken.Path.Contains("submoduleData") && jToken.Path.Contains("summaryHeader"))
                                    {
                                        List<Dictionary<string, object>> dataSheetList = (List<Dictionary<string, object>>)dataSheet[fullSubmoduleName];

                                        string calculationType = jObject["value"].ToObject<string>().ToLower().Trim();
                                        string name = $"{moduleName}.{submoduleName}.{submoduleName}{calculationType}";

                                        var lastDataSheet = dataSheetList[dataSheetList.Count - 1];
                                        if (lastDataSheet.ContainsKey(name))
                                        {
                                            lastDataSheet = new Dictionary<string, object>();
                                            if (jToken.Path.Contains("currentSubmodule"))
                                            {
                                                lastDataSheet.Add("currentsubmodule", true);
                                            }
                                            else
                                            {
                                                lastDataSheet.Add("currentsubmodule", false);
                                            }
                                            dataSheetList.Add(lastDataSheet);
                                        }

                                        if (!lastDataSheet.ContainsKey(name))
                                        {
                                            lastDataSheet.Add(name, null);
                                        }
                                    }
                                }
                                else if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    jObject.Properties().Any(prop => prop.Name == "title") &&
                                    jObject.Properties().Any(prop => prop.Name == "inputs"))
                                {
                                    if (!jToken.Path.Contains("submoduleData"))
                                    {
                                        break;
                                    }

                                    submoduleName = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                    fullSubmoduleName = $"{moduleName}.{submoduleName}";

                                    if (!dataSheet.ContainsKey(fullSubmoduleName))
                                    {
                                        var dataSheetList = new List<Dictionary<string, object>>() { new Dictionary<string, object>() };
                                        if (jToken.Path.Contains("currentSubmodule"))
                                        {
                                            dataSheetList[0].Add("currentsubmodule", true);
                                        }
                                        else
                                        {
                                            dataSheetList[0].Add("currentsubmodule", false);
                                        }
                                        dataSheet.Add(fullSubmoduleName, dataSheetList);
                                    }

                                    if (!submoduleCount.ContainsKey(submoduleName))
                                    {
                                        submoduleCount.Add(submoduleName, 0);
                                    }
                                    else
                                    {
                                        submoduleCount[submoduleName]++;
                                    }
                                    submoduleList.Add((submoduleName, submoduleCount[submoduleName]));
                                }
                                else if (jObject.Properties().Any(prop => prop.Name == "number") &&
                                    jObject.Properties().Any(prop => prop.Name == "description"))
                                {
                                    if (!jToken.Path.Contains("submoduleData") || !jToken.Path.Contains("dataSummary"))
                                    {
                                        break;
                                    }

                                    int index = jObject["number"].ToObject<int>() - 1;
                                    (string summarySubmoduleName, int summarySubmoduleIndex) = submoduleList[index];
                                        
                                    var submodules = (List<Dictionary<string, object>>)dataSheet[$"{moduleName}.{summarySubmoduleName}"];
                                    var submodule = submodules[summarySubmoduleIndex];

                                    foreach (var property in jObject.Properties())
                                    {
                                        string parameterName = $"{moduleName}.{summarySubmoduleName}.{summarySubmoduleName}{property.Name}";

                                        object inputValue;
                                        switch (property.Type)
                                        {
                                            case JTokenType.Integer:
                                                {
                                                    decimal? value = Convert.ToDecimal(property.ToObject<int>());
                                                    inputValue = value;
                                                    break;
                                                }
                                            case JTokenType.Float:
                                                {
                                                    decimal? value = Convert.ToDecimal(property.ToObject<float>());
                                                    inputValue = value;
                                                    break;
                                                }
                                            case JTokenType.Boolean:
                                                {
                                                    bool? value = property.ToObject<bool>();
                                                    inputValue = value;
                                                    break;
                                                }
                                            default:
                                                {
                                                    string value = property.ToObject<string>();
                                                    inputValue = string.IsNullOrWhiteSpace(value) ? null : value;
                                                    break;
                                                }
                                        }

                                        submodule[parameterName] = inputValue;
                                    }
                                }
                                else if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    jObject.Properties().Any(prop => prop.Name == "tableItems"))
                                {
                                    string tableName = jObject["variableName"].ToObject<string>().ToLower();
                                    var tableItems = (JArray)jObject["tableItems"];
                                    foreach(var tableJToken in tableItems)
                                    {
                                        var tableJObject = (JObject)tableJToken;
                                        foreach(var prop in tableJObject.Properties())
                                        {
                                            dataSheet.Add($"{moduleName}.{tableName}{prop.Name.ToLower()}", null);
                                        }
                                        break;
                                    }
                                }
                                else if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    jObject.Properties().Any(prop => prop.Name == "dataObjects") &&
                                    jObject.Properties().Any(prop => prop.Name == "dataSummary"))
                                {
                                    if (!jToken.Path.Contains("submoduleData"))
                                    {
                                        break;
                                    }

                                    submoduleList.Clear();
                                }
                            }
                            break;
                    }
                });
        }

        private void ParseToJson(JObject uiInputs, Dictionary<string, object> dataSheet, string moduleName)
        {
            string submoduleName = string.Empty;
            string fullSubmoduleName = string.Empty;

            var submoduleList = new List<(string, int)>();
            var submoduleCount = new Dictionary<string, int>();
            int previousIndex = 0;
            uiInputs.WalkNodes(PreOrder: jToken =>
            {
                if (jToken.Path.Contains("moduleInputs") && jToken.Path.Contains("submoduleData"))
                {
                    return;
                }

                switch (jToken)
                {
                    case JObject jObject:
                        {
                            if (jObject.Properties().Any(prop => prop.Name == "inputType") && jObject["inputType"].ToObject<string>().ToLower() == "file input")
                            {
                                break;
                            }

                            if(jObject.Path.EndsWith("currentSubmodule"))
                            {
                                previousIndex++;
                            }

                            if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                (jObject.Properties().Any(prop => prop.Name == "inputValue") ||
                                jObject.Properties().Any(prop => prop.Name == "quantity")))
                            {
                                bool isCalculated = IsCalculated(jObject);

                                string name = jObject["variableName"].ToObject<string>().ToLower().Trim();

                                string parameterName;
                                Dictionary<string, object> dataSheetToUse;
                                if (jToken.Path.Contains("submoduleData"))
                                {
                                    parameterName = $"{moduleName}.{submoduleName}.{name}";
                                    var submodules = (List<Dictionary<string, object>>)dataSheet[$"{moduleName}.{submoduleName}"];
                                    (_, int submoduleIndex) = submoduleList.Last();
                                    dataSheetToUse = submodules[submoduleIndex];
                                }
                                else
                                {
                                    parameterName = $"{moduleName}.{name}";
                                    dataSheetToUse = dataSheet;
                                }

                                if (isCalculated && dataSheetToUse.TryGetValue(parameterName, out object value) && value != null)
                                {
                                    if (jObject.Properties().Any(prop => prop.Name == "inputType") && (jObject["inputType"].ToObject<string>().ToLower() == "select" || jObject["inputType"].ToObject<string>().ToLower() == "combobox") && value is object[])
                                    {
                                        var options = new List<Dictionary<string, string>>();
                                        foreach (var returnedOption in (object[])value)
                                        {
                                            var option = new Dictionary<string, string>();
                                            var optionsParts = (object[])returnedOption;
                                            option.Add("text", UnwrapString(optionsParts[0].ToString()));
                                            option.Add("value", UnwrapString(optionsParts[1].ToString()));
                                            options.Add(option);
                                        }

                                        if(options.Count == 2 && options[0]["text"] == "--No Selection--" && string.IsNullOrEmpty(options[0]["value"]))
                                        {
                                            options.RemoveAt(0);
                                        }

                                        jObject["options"] = JToken.FromObject(options);
                                        if (options.Count == 1)
                                        {
                                            jObject["inputValue"] = options[0]["value"];
                                        }
                                    }
                                    else
                                    {
                                        JToken valueJToken = JToken.FromObject(value);
                                        string jObjKey = jObject.Properties().Any(prop => prop.Name == "inputValue") ? "inputValue" : "quantity";

                                        switch (valueJToken.Type)
                                        {
                                            case JTokenType.String:
                                                {
                                                    jObject[jObjKey] = UnwrapString(valueJToken.ToObject<string>());
                                                }
                                                break;
                                            default:
                                                {
                                                    jObject[jObjKey] = valueJToken;
                                                }
                                                break;
                                        }
                                    }
                                }

                                if (jObject.Properties().Any(prop => prop.Name == "quantity"))
                                {
                                    string originalVariableName = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                    foreach (var prop in jObject.Properties())
                                    {
                                        if (prop.Name == "quantity")
                                        {
                                            continue;
                                        }

                                        if (prop.Value.Type != JTokenType.String || !string.IsNullOrEmpty(prop.Value.ToObject<string>()))
                                        {
                                            continue;
                                        }

                                        name = $"{originalVariableName}{prop.Name.ToLower().Trim()}";

                                        if (jToken.Path.Contains("submoduleData"))
                                        {
                                            parameterName = $"{moduleName}.{submoduleName}.{name}";
                                            var submodules = (List<Dictionary<string, object>>)dataSheet[$"{moduleName}.{submoduleName}"];
                                            (_, int submoduleIndex) = submoduleList.Last();
                                            dataSheetToUse = submodules[submoduleIndex];
                                        }
                                        else
                                        {
                                            parameterName = $"{moduleName}.{name}";
                                            dataSheetToUse = dataSheet;
                                        }

                                        if (dataSheetToUse.TryGetValue(parameterName, out value) && value != null)
                                        {
                                            JToken valueJToken = JToken.FromObject(value);
                                            string jObjKey = prop.Name;

                                            switch (valueJToken.Type)
                                            {
                                                case JTokenType.String:
                                                    {
                                                        jObject[jObjKey] = UnwrapString(valueJToken.ToObject<string>());
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        jObject[jObjKey] = valueJToken;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    jObject.Properties().Any(prop => prop.Name == "title") &&
                                    jObject.Properties().Any(prop => prop.Name == "inputs"))
                            {
                                if (!jToken.Path.Contains("submoduleData"))
                                {
                                    break;
                                }

                                submoduleName = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                fullSubmoduleName = $"{moduleName}.{submoduleName}";
                                if (!submoduleCount.ContainsKey(submoduleName))
                                {
                                    submoduleCount.Add(submoduleName, 0);
                                }
                                else
                                {
                                    submoduleCount[submoduleName]++;
                                }
                                submoduleList.Add((submoduleName, submoduleCount[submoduleName]));
                            }
                            else if (jObject.Properties().Any(prop => prop.Name == "number") &&
                                jObject.Properties().Any(prop => prop.Name == "description"))
                            {
                                if (!jToken.Path.Contains("submoduleData") || !jToken.Path.Contains("dataSummary"))
                                {
                                    break;
                                }

                                int index = previousIndex;
                                (string summarySubmoduleName, int summarySubmoduleIndex) = submoduleList[index];

                                var submodules = (List<Dictionary<string, object>>)dataSheet[$"{moduleName}.{summarySubmoduleName}"];
                                var submodule = submodules[summarySubmoduleIndex];

                                foreach (var property in jObject.Properties())
                                {
                                    if (property.Name == "number")
                                    {
                                        continue;
                                    }

                                    string parameterName = $"{moduleName}.{summarySubmoduleName}.{summarySubmoduleName}{property.Name}";
                                    if (submodule.TryGetValue(parameterName, out object value) && value != null)
                                    {
                                        var valueJToken = JToken.FromObject(value);
                                        switch (valueJToken.Type)
                                        {
                                            case JTokenType.String:
                                                {
                                                    property.Value = UnwrapString(valueJToken.ToObject<string>());
                                                }
                                                break;
                                            default:
                                                {
                                                    property.Value = valueJToken;
                                                }
                                                break;
                                        }
                                    }
                                }
                                previousIndex++;
                            }
                            else if (jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                jObject.Properties().Any(prop => prop.Name == "tableItems"))
                            {
                                string tableName = jObject["variableName"].ToObject<string>().ToLower();
                                JArray tableItems = (JArray)jObject["tableItems"];
                                var itemNames = new List<string>();
                                foreach (var tableJToken in tableItems)
                                {
                                    var tableJObject = (JObject)tableJToken;
                                    foreach (var prop in tableJObject.Properties())
                                    {
                                        itemNames.Add(prop.Name.ToLower());
                                    }
                                    break;
                                }
                                tableItems.Clear();
                                var items = new List<JObject>();
                                bool first = true;
                                foreach(var itemName in itemNames)
                                {
                                    var dsi = dataSheet[$"{moduleName}.{tableName}{itemName}"];
                                    if(dsi == null)
                                    {
                                        continue;
                                    }

                                    var dataSheetItems = new List<object>((IEnumerable<object>)dsi);
                                    if(dataSheetItems == null)
                                    {
                                        continue;
                                    }
                                    if(first)
                                    {
                                        foreach(var dataSheetItem in dataSheetItems)
                                        {
                                            var obj = new JObject();
                                            var valueJToken = JToken.FromObject(dataSheetItem);
                                            switch (valueJToken.Type)
                                            {
                                                case JTokenType.String:
                                                    {
                                                        obj.Add(itemName, UnwrapString(valueJToken.ToObject<string>()));
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        obj.Add(itemName, valueJToken);
                                                    }
                                                    break;
                                            }
                                            items.Add(obj);
                                        }
                                        first = false;
                                    }
                                    else
                                    {
                                        for(int i = 0; i < dataSheetItems.Count; ++i)
                                        {
                                            if(i >= items.Count)
                                            {
                                                items.Add(new JObject());
                                            }

                                            if (dataSheetItems[i] != null)
                                            {
                                                var valueJToken = JToken.FromObject(dataSheetItems[i]);
                                                switch (valueJToken.Type)
                                                {
                                                    case JTokenType.String:
                                                        {
                                                            items[i].Add(itemName, UnwrapString(valueJToken.ToObject<string>()));
                                                        }
                                                        break;
                                                    default:
                                                        {
                                                            items[i].Add(itemName, valueJToken);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach(var tableItem in items)
                                {
                                    tableItems.Add(tableItem);
                                }
                            }


                            break;
                        }
                }
            });
        }

        private static string UnwrapString(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if(str.StartsWith("$"))
            {
                return str.Substring(1);
            }
            return str;
        }

        private static bool IsCalculated(JObject jObject)
        {
            if (jObject.Properties().Any(prop => prop.Name == "input"))
            {
                bool isCalculated = !jObject["input"].ToObject<bool>();
                if(isCalculated)
                {
                    return true;
                }
            }
            if (jObject.Properties().Any(prop => prop.Name == "parent"))
            {
                bool isCalculated = jObject["parent"].ToObject<string>() != "None";
                if (!isCalculated)
                {
                    return false;
                }
            }
            if (jObject.Properties().Any(prop => prop.Name == "inputValue"))
            {
                switch(jObject["inputValue"].Type)
                {
                    case JTokenType.String:
                        {
                            bool isCalculated = string.IsNullOrEmpty(jObject["inputValue"].ToObject<string>());
                            if (isCalculated)
                            {
                                return true;
                            }
                        }
                        break;
                    case JTokenType.Object:
                        {
                            bool isCalculated = string.IsNullOrEmpty(((JObject)jObject["inputValue"])["value"].ToObject<string>());
                            if (isCalculated)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }
        
        public async Task<(object, string)> Clone(JObject document, string path) {
            path = path.Split('/').Last();
            document = JTokenExtension.WalkNode(document, path);
            return (await _estimateRepository.Clone(document), "OK");
        }


        public async Task<(object, string)> List(IDictionary<string, string> queryParams) {
            return (await _estimateRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams) {
            return (await _estimateRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document) {
            return (await _estimateRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams) {
            int deletedDocumentCount = await _estimateRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Import(string env) {
            string envSetting = $"{env}-url";
            string url = Environment.GetEnvironmentVariable(envSetting);
            if (url == null) {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/estimates/?object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200") {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _estimateRepository.Reset(jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }
    }
}
