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

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IDictionary<string, IFunction> _functions;
        private readonly ILogger _log;

        public EstimateService(IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, ILogger log)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _functions = Primitive.Load();
            _log = log;
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
                                if(!(jObject.Properties().Any(prop => prop.Name == "calculated") &&
                                    jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                    jObject.Properties().Any(prop => prop.Name == "inputValue")))
                                {
                                    break;
                                }

                                bool isCalculated = jObject["calculated"].ToObject<bool>();
                                string name = jObject["variableName"].ToObject<string>().ToLower().Trim();
                                object inputValue;
                                JToken inputValueFromJson = jObject["inputValue"];
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

                                dataSheet.Add($"{moduleName}.{name}", isCalculated ? inputValue : null);
                                break;
                            }
                    }
                });

            var rulesEngine = new RulesEngine();
            rulesEngine.EvaluateDataSheet(dataSheet, DateTime.Now, _functions, _instructionSetRepository, _referenceDataRepository, _log);

            //Convert back
            uiInputs.WalkNodes(PreOrder: jToken =>
            {
                switch (jToken)
                {
                    case JObject jObject:
                        {
                            if (!(jObject.Properties().Any(prop => prop.Name == "calculated") &&
                                jObject.Properties().Any(prop => prop.Name == "variableName") &&
                                jObject.Properties().Any(prop => prop.Name == "inputValue")))
                            {
                                break;
                            }

                            bool isCalculated = jObject["calculated"].ToObject<bool>();
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
                                        option.Add("text", optionsParts[0].ToString());
                                        option.Add("value", optionsParts[1].ToString());
                                        options.Add(option);
                                    }

                                    jObject["options"] = JToken.FromObject(options);
                                }
                                else
                                {
                                    jObject["inputValue"] = JToken.FromObject(value);
                                }
                                break;
                            }

                            break;
                        }
                }
            });

            return (uiInputs, "");
        }
    }
}
