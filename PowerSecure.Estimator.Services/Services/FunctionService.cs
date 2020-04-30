using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
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

        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.List(queryParams),"OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.Get(id, queryParams),"OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            if (!document.ContainsKey("creationdate"))
            {
                document.Add("creationdate", JToken.FromObject(DateTime.Now.ToString("M/d/yyyy")));
            }
            if(document.ContainsKey("module"))
            {
                document["module"] = document["module"].ToString().ToLower();
            }
            if (document.ContainsKey("name"))
            {
                document["name"] = document["name"].ToString().ToLower();
            }
            return (await _functionRepository.Upsert(document),"OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _functionRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Import(string env, string module)
        {
            string envSetting = $"{env}-url";
            string url = Environment.GetEnvironmentVariable(envSetting);
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

        public async Task<(object, string)> UpsertFromUi(string requestBody)
        {
            (object obj, string str) = await UpsertFromRequestBody(requestBody);

            if (obj == null)
            {
                return (obj, str);
            }

            JObject requestBodyJObject = JObject.Parse(requestBody);
            JObject instructionSet = JObject.FromObject(obj);

            if (!requestBodyJObject.ContainsKey("id"))
            {
                requestBodyJObject.Add("id", instructionSet["id"].ToString());
            }

            if(instructionSet.ContainsKey("uijson"))
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
            instructionSetJObject.Add("startdate", DateTime.ParseExact(requestJObject["effectiveDate"].ToString(),"yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("M/d/yyyy"));

            if (!requestJObject.ContainsKey("calculatedVariable"))
            {
                return (null, "Invalid instruction set - no calculatedVariable");
            }
            {
                var jObject = (JObject)requestJObject["calculatedVariable"];
                if(!jObject.ContainsKey("variableName") || string.IsNullOrWhiteSpace(jObject["variableName"].ToString()))
                {
                    return (null, "Invalid instruction set - no variableName");
                }

                instructionSetJObject.Add("name", jObject["variableName"]);
            }

            {
                var finalSets = new List<JObject>();
                requestJObject.WalkNodes(PreOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                if(jObject.ContainsKey("final") && (bool)jObject["final"])
                                {
                                    finalSets.Add(jObject);
                                }
                                break;
                            }
                    }
                });

                if(finalSets.Count == 0)
                {
                    return (null, "Invalid instruction set - no final set");
                }

                if(finalSets.Count > 1)
                {
                    return (null, "Invalid instruction set - more than one final set");
                }

                var finalSet = finalSets[0];
                int firstId = int.MinValue;
                Dictionary<int, (string, List<object>)> dict = new Dictionary<int, (string, List<object>)>();
                finalSet.WalkNodes(PreOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                if (!jObject.ContainsKey("id"))
                                {
                                    break;
                                }

                                int id = jObject["id"].ToObject<int>();
                                if(firstId == int.MinValue)
                                {
                                    firstId = id;
                                }
                                if (dict.ContainsKey(id))
                                {
                                    break;
                                }

                                ParseInstructionSetsToDictionary(jObject, dict);
                                break;
                            }
                    }
                },
                PostOrder: jToken =>
                {
                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                if (!jObject.ContainsKey("id"))
                                {
                                    break;
                                }

                                int id = jObject["id"].ToObject<int>();

                                if(dict[id].Item2 == null)
                                {
                                    break;
                                }

                                ResolveInstructionSetsFromDictionary(id, dict);
                                break;
                            }
                    }
                });

                instructionSetJObject.Add("instructions", dict[firstId].Item1);
            }

            return await Upsert(instructionSetJObject);
        }

        private void ParseInstructionSetsToDictionary(JObject jObject, Dictionary<int, (string, List<object>)> dict)
        {
            int id = jObject["id"].ToObject<int>();
            string primitive = ((JObject)jObject["instructionMethod"])["v"].ToString();
            JArray instructionParamsJArray = (JArray)jObject["instructionParams"];
            var instructionParams = new List<object>();
            foreach (JToken instructionParamToken in instructionParamsJArray)
            {
                if (instructionParamToken is JObject instructionParam)
                {
                    if (instructionParam.ContainsKey("value"))
                    {
                        JToken valueToken = instructionParam["value"];
                        switch (valueToken.Type)
                        {
                            case JTokenType.Null:
                                break;
                            case JTokenType.Object:
                                {
                                    JObject valueObj = (JObject)valueToken;
                                    if(valueObj.ContainsKey("id"))
                                    {
                                        instructionParams.Add(new UnresolvedSet() { Id = valueObj["id"].ToObject<int>() });
                                    }
                                    else
                                    {
                                        instructionParams.Add($"{valueObj["moduleTitle"].ToString().ToLower()}.{valueObj["variableName"].ToString().ToLower()}");
                                    }
                                    break;
                                }
                            default:
                                {
                                    instructionParams.Add(valueToken);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        instructionParams.Add(instructionParam);
                    }
                }
            }

            dict.Add(id, (primitive.ToLower(), instructionParams));
        }

        private void ResolveInstructionSetsFromDictionary(int currentId, Dictionary<int, (string, List<object>)> dict)
        {
            (string primitive, List<object> parameters) = dict[currentId];
            for(int i = 0; i < parameters.Count; ++i)
            {
                switch(parameters[i])
                {
                    case UnresolvedSet unresolvedSet:
                        {
                            parameters[i] = dict[unresolvedSet.Id].Item1;
                            break;
                        }
                    case JObject jObject:
                        {
                            if(primitive != "find")
                            {
                                break;
                            }

                            StringBuilder str = new StringBuilder();
                            str.Append("\"$Factor\",[");
                            bool first = true;
                            string moduleName = jObject["module"].ToString();
                            foreach(var pair in jObject)
                            {
                                if(pair.Key == "returnattribute" ||
                                   pair.Key == "returnvalue" ||
                                   string.IsNullOrWhiteSpace(pair.Value.ToString()))
                                {
                                    continue;
                                }
                                if(first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    str.Append(",");
                                }

                                if(pair.Value is JObject variableInput)
                                {
                                    str.Append($"[\"${pair.Key.ToLower()}\",\"{variableInput["moduleTitle"].ToString().ToLower()}.{variableInput["variableName"].ToString().ToLower()}\"]");
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
                                str.Append($"\"{returnAttributeInput["moduleTitle"].ToString().ToLower()}.{returnAttributeInput["variableName"].ToString().ToLower()}\"");
                            }
                            else
                            {
                                str.Append($"\"${returnAttribute.ToString().ToLower()}\"");
                            }

                            parameters[i] = str.ToString();

                            break;
                        }
                    case JToken jToken:
                        {
                            parameters[i] = jToken.ToString();
                            break;
                        }
                }
            }

            if(primitive == "#")
            {
                dict[currentId] = (parameters[0].ToString(), null);
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append($"{{\"{primitive}\":[");
                bool first = true;
                foreach(object parameter in parameters)
                {
                    if(first)
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
                dict[currentId] = (str.ToString(), null);
            }
        }

        private class UnresolvedSet { public int Id { get; set; } }
        
        public async Task<(object, string)> GetFromUi(string id)
        {
            return await Get(id, new Dictionary<string,string>());
        }

        public async Task<(object, string)> ListPrimitives()
        {
            return (new List<Dictionary<string, string>>()
            {
                new Dictionary<string,string>(){ ["label"] = "Add", ["value"] = "+", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "And", ["value"] = "and", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Ceiling", ["value"] = "ceiling", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Check", ["value"] = "if", ["maxParams"] = "3", ["minParams"] = "3" },
                new Dictionary<string,string>(){ ["label"] = "Concatenate", ["value"] = "str", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Division", ["value"] = "/", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Equal", ["value"] = "=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Find", ["value"] = "find", ["maxParams"] = "4", ["minParams"] = "4" },
                new Dictionary<string,string>(){ ["label"] = "Floor", ["value"] = "floor", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "GreaterThan", ["value"] = ">", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "GreaterThanOrEqual", ["value"] = ">=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "IsEmpty", ["value"] = "isempty", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "LessThan", ["value"] = "<", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "LessThanOrEqual", ["value"] = "<=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "List", ["value"] = "list", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Margin", ["value"] = "margin", ["maxParams"] = "3", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Max", ["value"] = "max", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Min", ["value"] = "min", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "MinusOne", ["value"] = "--", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Multiply", ["value"] = "*", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "NotEqual", ["value"] = "!=", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Not", ["value"] = "not", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Or", ["value"] = "or", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Power", ["value"] = "^", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Price", ["value"] = "price", ["maxParams"] = "4", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Step", ["value"] = "++", ["maxParams"] = "1", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Subtract", ["value"] = "-", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "SumProduct", ["value"] = "sumproduct", ["maxParams"] = "2", ["minParams"] = "2" },
                new Dictionary<string,string>(){ ["label"] = "Switch", ["value"] = "switch", ["maxParams"] = "none", ["minParams"] = "1" },
                new Dictionary<string,string>(){ ["label"] = "Threshold", ["value"] = "threshold", ["maxParams"] = "4", ["minParams"] = "4" },
                new Dictionary<string,string>(){ ["label"] = "Zero", ["value"] = "zero", ["maxParams"] = "1", ["minParams"] = "1" }
            }, "OK");
        }
    }
}
