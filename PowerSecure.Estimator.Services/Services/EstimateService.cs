using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSecure.Estimator.Services.Components.RulesEngine;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IFunctionRepository _functionRepository;

        public EstimateService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<(object, string)> Evaluate(JObject uiInputs)
        {
            var dataSheet = new Dictionary<string, object>();
            string moduleName = uiInputs.Properties().Where(prop => prop.Name == "moduleTitle").First().Value.ToObject<string>().ToLower().Trim();
            //TODO - Translate into data sheet
            uiInputs.WalkNodes(PreOrder: jToken =>
                {
                    switch(jToken)
                    {
                        case JObject jObject:
                            {
                                if(!jObject.Properties().Any(prop => prop.Name == "input"))
                                {
                                    break;
                                }

                                bool isInput = jObject.Properties().Where(prop => prop.Name == "input").First().Value.ToObject<bool>();
                                string name = jObject.Properties().Where(prop => prop.Name == "variableName").First().Value.ToObject<string>().ToLower().Trim();
                                object inputValue;
                                JToken inputValueFromJson = jObject.Properties().Where(prop => prop.Name == "inputValue").First().Value;
                                switch(inputValueFromJson.Type)
                                {
                                    case JTokenType.Integer:
                                        {
                                            inputValue = Convert.ToDecimal(inputValueFromJson.ToObject<int>());
                                            break;
                                        }
                                    case JTokenType.Float:
                                        {
                                            inputValue = Convert.ToDecimal(inputValueFromJson.ToObject<float>());
                                            break;
                                        }
                                    case JTokenType.Boolean:
                                        {
                                            inputValue = inputValueFromJson.ToObject<bool>();
                                            break;
                                        }
                                    default:
                                        {
                                            inputValue = inputValueFromJson.ToObject<string>();
                                            break;
                                        }
                                }

                                dataSheet.Add($"{moduleName}.{name}", isInput ? inputValue : null);
                                break;
                            }
                    }
                });

            //TODO - Add functionality
            return (null, "");
        }
    }
}
