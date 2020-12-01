using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class FunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly ILogger _log;

        public FunctionService(IFunctionRepository functionRepository, ILogger log)
        {
            _functionRepository = functionRepository;
            _log = log;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            if(queryParams.ContainsKey("name") && !queryParams.ContainsKey("module"))
            {
                queryParams.Add("module", queryParams["name"].Substring(0, queryParams["name"].IndexOf(".")));
                queryParams["name"] = queryParams["name"].Substring(queryParams["name"].IndexOf(".") + 1);
            }
            
            var list = (List<Function>)await _functionRepository.List(queryParams);

            bool reportUiJson = !((queryParams.TryGetValue("uijson", out string value) && value.ToLower() == "false"));

            foreach (var function in list)
            {
                if(function.Rest != null && !function.Rest.ContainsKey("startdate"))
                {
                    _log.LogWarning($"Function {function.Name} with id {function.Id} does not contain a start date.");
                    continue;
                }

                if (function.Rest != null && !function.Rest.ContainsKey("instructions"))
                {
                    _log.LogWarning($"Function {function.Name} with id {function.Id} does not contain instructions.");
                    continue;
                }

                if (function.Rest != null)
                {
                    if(reportUiJson && !function.Rest.ContainsKey("uijson"))
                    {
                        var dict = new Dictionary<string, object>()
                        {
                            ["moduleTitle"] = function.Module,
                            ["effectiveDate"] = DateTime.Parse(function.Rest["startdate"].ToString()).ToString("yyyy-MM-dd"),
                            ["calculatedVariable"] = new Dictionary<string, object>()
                            {
                                ["variableName"] = function.Name,
                                ["moduleTitle"] = function.Module
                            },
                            ["instructionSets"] = new List<object>()
                        {
                            new Dictionary<string,object>()
                            {
                                ["id"] = 1,
                                ["instructionMethod"] = new Dictionary<string,object>()
                                {
                                    ["label"] = "Json",
                                    ["v"] = "json",
                                    ["maxParams"] = "1",
                                    ["minParams"] = "1"
                                },
                                ["instructionParams"] = new List<object>()
                                {
                                    new Dictionary<string,object>()
                                    {
                                        ["json"] = function.Rest["instructions"].ToString(),
                                        ["moduleTitle"] = function.Module
                                    }
                                },
                                ["final"] = true
                            }
                        },
                            ["id"] = function.Id
                        };

                        function.Rest.Add("uijson", dict);
                    }
                    else if (!reportUiJson && function.Rest.ContainsKey("uijson"))
                    {
                        function.Rest.Remove("uijson");
                    }
                }
            }

            return (list, "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            if (!document.ContainsKey("creationdate"))
            {
                document.Add("creationdate", JToken.FromObject(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")));
            }

            if (document.ContainsKey("startdate"))
            {
                document["startdate"] = DateTime.Parse(document["startdate"].ToString()).ToString("yyyy-MM-ddT00:00:00");
            }
            else
            {
                return (null, "Function is lacking a startdate property");
            }

            if (document.ContainsKey("module"))
            {
                document["module"] = document["module"].ToString().ToLower();
            }
            else
            {
                return (null, "Function is lacking a module property");
            }

            if (document.ContainsKey("name"))
            {
                document["name"] = document["name"].ToString().ToLower();
            }
            else
            {
                return (null, "Function is lacking a name property");
            }


            if (!document.ContainsKey("instructions"))
            {
                return (null, "Function is lacking an instructions property");
            }

            return (await _functionRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _functionRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Reset(string env, string module)
        {
            string envSetting = $"{env}-url";
            string url = AppSettings.Get(envSetting);
            if (url == null)
            {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/functions/?module={module}&object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _functionRepository.Reset(module, jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }

        public async Task<(object, string)> Import(string env, string module)
        {
            string envSetting = $"{env}-url";
            string url = AppSettings.Get(envSetting);
            if (url == null)
            {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/functions/?module={module}&object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _functionRepository.Import(jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents updated.");
        }

        public async Task<(object, string)> UpsertFromUi(string requestBody)
        {
            (object obj, string str) = await UpsertFromRequestBody(requestBody);

            if (obj == null)
            {
                return (obj, str);
            }

            JObject requestBodyJObject = JObject.Parse(requestBody);
            JObject instructionSet = JObject.FromObject(obj);

            if (!requestBodyJObject.ContainsKey("id") || requestBodyJObject["id"] == null || string.IsNullOrEmpty(requestBodyJObject["id"].ToString()))
            {
                requestBodyJObject.Add("id", instructionSet["id"].ToString());
            }

            if (instructionSet.ContainsKey("uijson"))
            {
                instructionSet["uijson"] = requestBodyJObject;
            }
            else
            {
                instructionSet.Add("uijson", requestBodyJObject);
            }

            return await Upsert(instructionSet);
        }

        private async Task<(object, string)> UpsertFromRequestBody(string requestBody)
        {
            var requestJObject = JObject.Parse(requestBody);
            var instructionSetJObject = new JObject();

            if (requestJObject.ContainsKey("id"))
            {
                instructionSetJObject.Add("id", requestJObject["id"]);
            }

            if (!requestJObject.ContainsKey("moduleTitle") || string.IsNullOrWhiteSpace(requestJObject["moduleTitle"].ToString()))
            {
                return (null, "Invalid instruction set - no module title");
            }
            instructionSetJObject.Add("module", requestJObject["moduleTitle"]);

            if (!requestJObject.ContainsKey("effectiveDate") || string.IsNullOrWhiteSpace(requestJObject["effectiveDate"].ToString()))
            {
                return (null, "Invalid instruction set - no effective date");
            }
            instructionSetJObject.Add("startdate", DateTime.ParseExact(requestJObject["effectiveDate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("M/d/yyyy"));

            if (!requestJObject.ContainsKey("calculatedVariable"))
            {
                return (null, "Invalid instruction set - no calculatedVariable");
            }
            {
                var jObject = (JObject)requestJObject["calculatedVariable"];
                if (!jObject.ContainsKey("variableName") || string.IsNullOrWhiteSpace(jObject["variableName"].ToString()))
                {
                    return (null, "Invalid instruction set - no variableName");
                }

                instructionSetJObject.Add("name", jObject["variableName"]);
            }

            {
                var instructionSequences = new List<JObject>(((JArray)requestJObject["instructionSets"]).Select(x => (JObject)x));

                var finalIndexes = instructionSequences.Where(jObj => jObj.ContainsKey("final") && (bool)jObj["final"]).Select(jObj => jObj["id"].ToObject<int>()).ToList();
                
                if (finalIndexes.Count == 0)
                {
                    return (null, "Invalid instruction set - no final set");
                }

                if (finalIndexes.Count > 1)
                {
                    return (null, "Invalid instruction set - more than one final set");
                }

                var dict = new SortedDictionary<int, (string, List<object>)>();
                foreach (var jObject in instructionSequences)
                {
                    ParseInstructionSetsToDictionary(jObject, dict);
                }

                ResolveInstructionSetsFromDictionary(finalIndexes[0], dict);

                instructionSetJObject.Add("instructions", dict[finalIndexes[0]].Item1);
            }

            return await Upsert(instructionSetJObject);
        }

        private void ParseInstructionSetsToDictionary(JObject jObject, IDictionary<int, (string, List<object>)> dict)
        {
            int id = jObject["id"].ToObject<int>();
            string primitive = ((JObject)jObject["instructionMethod"])["v"].ToString();
            JArray instructionParamsJArray = (JArray)jObject["instructionParams"];
            var instructionParams = new List<object>();
            foreach (JToken instructionParamToken in instructionParamsJArray)
            {
                if (instructionParamToken is JObject instructionParam)
                {
                    object value = ParseValueFromInstructionParam(instructionParam);
                    if (value != null)
                    {
                        instructionParams.Add(value);
                    }
                }
            }

            dict.Add(id, (primitive.ToLower(), instructionParams));
        }

        private object ParseValueFromInstructionParam(JObject instructionParam)
        {
            if (instructionParam.ContainsKey("number"))
            {
                return decimal.Parse(instructionParam["number"].ToString());
            }
            else if (instructionParam.ContainsKey("string"))
            {
                return instructionParam["string"];
            }
            else if (instructionParam.ContainsKey("boolean"))
            {
                return bool.Parse(instructionParam["boolean"].ToString());
            }
            else if (instructionParam.ContainsKey("value"))
            {
                JToken valueToken = instructionParam["value"];
                switch (valueToken.Type)
                {
                    case JTokenType.Null:
                        return null;
                    case JTokenType.Object:
                        {
                            JObject valueObj = (JObject)valueToken;
                            if (valueObj.ContainsKey("id"))
                            {
                                return new UnresolvedSet() { Id = valueObj["id"].ToObject<int>() };
                            }
                            else
                            {
                                return valueObj["variableName"].ToString().ToLower();
                            }
                        }
                    default:
                        return valueToken;
                }
            }
            else if (instructionParam.ContainsKey("id"))
            {
                return new UnresolvedSet() { Id = instructionParam["id"].ToObject<int>() };
            }
            else if (instructionParam.ContainsKey("moduleTitle") && instructionParam.ContainsKey("variableName"))
            {
                return instructionParam["variableName"].ToString().ToLower();
            }
            else if (instructionParam.ContainsKey("moduleTitle") && instructionParam.ContainsKey("name"))
            {
                return instructionParam["name"].ToString().ToLower();
            }
            else
            {
                return instructionParam;
            }
        }

        private void ResolveInstructionSetsFromDictionary(int currentId, IDictionary<int, (string, List<object>)> dict)
        {
            (string primitive, List<object> parameters) = dict[currentId];

            if(parameters == null)
            {
                return;
            }

            for (int i = 0; i < parameters.Count; ++i)
            {
                string resolvedParameter = ResolveParameter(parameters[i], primitive, dict);
                if (resolvedParameter != null)
                {
                    parameters[i] = resolvedParameter;
                }
            }

            {
                StringBuilder str = new StringBuilder();
                switch(primitive)
                {
                    case "instructionset":
                    case "input":
                        {
                            str.Append(parameters[0].ToString());
                        }
                        break;
                    case "json":
                        {
                            str.Append(JObject.Parse(JObject.Parse(parameters[0].ToString())["json"].ToString()).ToString(Newtonsoft.Json.Formatting.None));
                        }
                        break;
                    default:
                        {
                            str.Append($"{{\"{primitive}\":[");
                            bool first = true;
                            foreach (object parameter in parameters)
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    str.Append(",");
                                }
                                str.Append(parameter.ToString());
                            }
                            str.Append("]}");
                        }
                        break;
                }

                dict[currentId] = (str.ToString(), null);
            }
        }

        private string ResolveParameter(object parameter, string primitive, IDictionary<int, (string, List<object>)> dict)
        {
            switch (parameter)
            {
                case UnresolvedSet unresolvedSet:
                    {
                        ResolveInstructionSetsFromDictionary(unresolvedSet.Id, dict);
                        return dict[unresolvedSet.Id].Item1;
                    }
                case JObject jObject:
                    {
                        switch (primitive)
                        {
                            case "find":
                                {
                                    StringBuilder str = new StringBuilder();
                                    str.Append("\"$Factor\",[");
                                    bool first = true;
                                    string moduleName = jObject["module"].ToString();
                                    foreach (var pair in jObject)
                                    {
                                        if (pair.Key == "returnattribute" ||
                                           pair.Key == "returnvalue" ||
                                           string.IsNullOrWhiteSpace(pair.Value.ToString()))
                                        {
                                            continue;
                                        }
                                        if (first)
                                        {
                                            first = false;
                                        }
                                        else
                                        {
                                            str.Append(",");
                                        }

                                        if (pair.Value is JObject variableInput)
                                        {
                                            str.Append($"[\"${pair.Key.ToLower()}\",{ResolveParameter(ParseValueFromInstructionParam(variableInput), primitive, dict)}]");
                                        }
                                        else
                                        {
                                            str.Append($"[\"${pair.Key.ToLower()}\",\"${pair.Value.ToString().ToLower()}\"]");
                                        }
                                    }
                                    str.Append("],\"All.EffectiveDate\",");

                                    JToken returnAttribute = jObject["returnattribute"];
                                    if (returnAttribute is JObject returnAttributeInput)
                                    {
                                        str.Append($"{ResolveParameter(ParseValueFromInstructionParam(returnAttributeInput),primitive,dict)}");
                                    }
                                    else
                                    {
                                        str.Append($"\"${returnAttribute.ToString().ToLower()}\"");
                                    }

                                    return str.ToString();
                                }
                            case "switch":
                                {
                                    StringBuilder str = new StringBuilder();
                                    string truthValue = ResolveParameter(ParseValueFromInstructionParam((JObject)jObject["switchOn"]), primitive, dict) ?? "null";
                                    str.Append($"{truthValue},[");
                                    bool first = true;
                                    JArray casePairs = (JArray)jObject["cases"];
                                    foreach (var casePairJToken in casePairs)
                                    {
                                        JObject casePair = (JObject)casePairJToken;
                                        string matchValue = ResolveParameter(ParseValueFromInstructionParam((JObject)casePair["case"]), primitive, dict);
                                        string returnValue = ResolveParameter(ParseValueFromInstructionParam((JObject)casePair["return"]), primitive, dict) ?? "null";

                                        if (matchValue != null)
                                        {
                                            if (first)
                                            {
                                                first = false;
                                            }
                                            else
                                            {
                                                str.Append(",");
                                            }

                                            str.Append($"[{matchValue},{returnValue}]");
                                        }
                                    }

                                    string defaultValue = ResolveParameter(ParseValueFromInstructionParam((JObject)jObject["defaultCase"]), primitive, dict) ?? "null";

                                    str.Append($"],{defaultValue}");

                                    return str.ToString();
                                }
                        }

                        break;
                    }
                case JToken jToken:
                    {
                        if (jToken.Type == JTokenType.String)
                        {
                            return $"\"${jToken.ToString()}\"";
                        }
                        else
                        {
                            return jToken.ToString();
                        }
                    }
                case string str:
                    {
                        return $"\"{str}\"";
                    }
                case decimal d:
                    {
                        return d.ToString();
                    }
                case bool b:
                    {
                        return b.ToString().ToLower();
                    }
            }

            return null;
        }

        private class UnresolvedSet { public int Id { get; set; } }

        public async Task<(object, string)> GetFromUi(string id)
        {
            return await Get(id, new Dictionary<string, string>());
        }

        private readonly List<Dictionary<string, string>> primitives = new List<Dictionary<string, string>>()
            {
                new Dictionary<string,string>(){ ["label"] = "Add", ["value"] = "+", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "And", ["value"] = "and", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Bool", ["value"] = "bool", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Ceiling", ["value"] = "ceiling", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Check", ["value"] = "if", ["maxParams"] = "3", ["minParams"] = "3" },
                new Dictionary<string,string>(){ ["label"] = "Concatenate", ["value"] = "str", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Contains", ["value"] = "contains", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Currency", ["value"] = "curr", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Division", ["value"] = "/", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Equal", ["value"] = "=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Find", ["value"] = "find", ["maxParams"] = "4", ["minParams"] = "4" },
                new Dictionary<string,string>(){ ["label"] = "Filter", ["value"] = "filter", ["maxParams"] = "3", ["minParams"] = "3" },
                new Dictionary<string,string>(){ ["label"] = "Floor", ["value"] = "floor", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Flatten", ["value"] = "flatten", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Guard", ["value"] = "guard", ["maxParams"] = "2", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "GreaterThan", ["value"] = ">", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "GreaterThanOrEqual", ["value"] = ">=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Identity", ["value"] = "identity", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "InstructionSet", ["value"] = "instructionSet", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Input", ["value"] = "input", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "IsEmpty", ["value"] = "isempty", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "IsFalse", ["value"] = "isfalse", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "IsTrue", ["value"] = "istrue", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Item", ["value"] = "item", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Json", ["value"] = "json", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Length", ["value"] = "len", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "LessThan", ["value"] = "<", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "LessThanOrEqual", ["value"] = "<=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "List", ["value"] = "list", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "LowerCase", ["value"] = "lcase", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Margin", ["value"] = "margin", ["maxParams"] = "3", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Map", ["value"] = "map", ["maxParams"] = "3", ["minParams"] = "3" },
                new Dictionary<string,string>(){ ["label"] = "Max", ["value"] = "max", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Min", ["value"] = "min", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "MinusOne", ["value"] = "--", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Modulus", ["value"] = "mod", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Multiply", ["value"] = "*", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "NumericFormat", ["value"] = "format", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "NotEqual", ["value"] = "!=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Not", ["value"] = "not", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Or", ["value"] = "or", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Percent", ["value"] = "percent", ["maxParams"] = "2", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Power", ["value"] = "^", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Price", ["value"] = "price", ["maxParams"] = "3", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Sequence", ["value"] = "seq", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Split", ["value"] = "split", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Step", ["value"] = "++", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "SubString", ["value"] = "substr", ["maxParams"] = "3", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Subtract", ["value"] = "-", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "SumProduct", ["value"] = "sumproduct", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Switch", ["value"] = "switch", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Threshold", ["value"] = "threshold", ["maxParams"] = "4", ["minParams"] = "4" },
                new Dictionary<string,string>(){ ["label"] = "UpperCase", ["value"] = "ucase", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Zero", ["value"] = "zero", ["maxParams"] = "1", ["minParams"] = "1" }
            };

        public async Task<(object, string)> ListPrimitives()
        {
            return (primitives, "OK");
        }
    }
}
