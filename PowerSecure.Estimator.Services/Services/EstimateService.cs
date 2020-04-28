using Newtonsoft.Json.Linq;
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

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IDictionary<string, IFunction> _functions;
        private readonly ILogger _log;
        private readonly IEstimateRepository _estimateRepository;

        public EstimateService(IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _functions = Primitive.Load();
            _log = log;
        }

        public EstimateService(IEstimateRepository estimateRepository) {
            _estimateRepository = estimateRepository;
        }

        public async Task<(object, string)> Evaluate(JObject uiInputs)
        {
            var dataSheet = new Dictionary<string, object>();
            string moduleName = uiInputs.Properties().Where(prop => prop.Name == "moduleTitle").First().Value.ToObject<string>().ToLower().Trim();

            //Translate into data sheet
            uiInputs.WalkNodes(PreOrder: jToken =>
                {
                    switch(jToken)
                    {
                        case JObject jObject:
                            {
                                if(!(jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    (jObject.Properties().Any(prop => prop.Name == "inputValue") ||
                                    jObject.Properties().Any(prop => prop.Name == "quantity"))))
                                {
                                    break;
                                }

                                bool isCalculated = IsCalculated(jObject);

                                string name = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                object inputValue;
                                JToken inputValueFromJson = jObject.Properties().Any(prop => prop.Name == "inputValue") ? jObject["inputValue"] : jObject["quantity"];
                                switch(inputValueFromJson.Type)
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

                                if (name.Contains('.'))
                                {
                                    dataSheet.Add($"{name}", isCalculated ? null : inputValue);
                                }
                                else
                                {
                                    dataSheet.Add($"{moduleName}.{name}", isCalculated ? null : inputValue);
                                }
                                break;
                            }
                    }
                });
            var rulesEngine = new RulesEngine();
            if(!dataSheet.ContainsKey("all.effectivedate"))
            {
                dataSheet.Add("all.effectivedate", DateTime.Now.ToString("M/d/yyyy"));
            }

            _log.LogInformation("Data sheet to calculate: " + JToken.FromObject(dataSheet));
            rulesEngine.EvaluateDataSheet(dataSheet, DateTime.Now, _functions, _instructionSetRepository, _referenceDataRepository, _log);
            _log.LogInformation("Returned data sheet: " + JToken.FromObject(dataSheet));

            //Convert back
            uiInputs.WalkNodes(PreOrder: jToken =>
            {
                switch (jToken)
                {
                    case JObject jObject:
                        {
                            if (!(jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                (jObject.Properties().Any(prop => prop.Name == "inputValue") ||
                                jObject.Properties().Any(prop => prop.Name == "quantity"))))
                            {
                                break;
                            }

                            bool isCalculated = IsCalculated(jObject);

                            string name = jObject["variableName"].ToObject<string>().ToLower().Trim();

                            if(isCalculated && dataSheet.TryGetValue($"{moduleName}.{name}", out object value) && value != null)
                            {
                                if (jObject.Properties().Any(prop => prop.Name == "inputType") && jObject["inputType"].ToObject<string>().ToLower() == "select" && value is object[])
                                {
                                    var options = new List<Dictionary<string, string>>();
                                    foreach(var returnedOption in (object[])value)
                                    {
                                        var option = new Dictionary<string, string>();
                                        var optionsParts = (object[])returnedOption;
                                        option.Add("text", UnwrapString(optionsParts[0].ToString()));
                                        option.Add("value", UnwrapString(optionsParts[1].ToString()));
                                        options.Add(option);
                                    }

                                    jObject["options"] = JToken.FromObject(options);
                                    if(options.Count == 1)
                                    {
                                        jObject["inputValue"] = options[0]["value"];
                                    }
                                }
                                else
                                {
                                    JToken valueJToken = JToken.FromObject(value);
                                    string jObjKey = jObject.Properties().Any(prop => prop.Name == "inputValue") ? "inputValue" : "quantity";

                                    switch(valueJToken.Type)
                                    {
                                        case JTokenType.String:
                                            {
                                                jObject[jObjKey] = UnwrapString(valueJToken.ToObject<string>());
                                            }
                                            break;
                                        case JTokenType.Float:
                                            {
                                                jObject[jObjKey] = Math.Round(valueJToken.ToObject<decimal>()-0.005m, 2);
                                            }
                                            break;
                                        default:
                                            {
                                                jObject[jObjKey] = valueJToken;
                                            }
                                            break;
                                    }
                                }
                                break;
                            }

                            break;
                        }
                }
            });

            return (uiInputs, "");
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
                bool isCalculated = string.IsNullOrEmpty(jObject["inputValue"].ToObject<string>());
                if (isCalculated)
                {
                    return true;
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
