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

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IDictionary<string, IFunction> _functions;

        public EstimateService(IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _functions = Primitive.Load();
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
                                if(!jObject.Properties().Any(prop => prop.Name == "calculated"))
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

            //TODO - Add evaluate functionality
            var rulesEngine = new RulesEngine();
            rulesEngine.EvaluateDataSheet(dataSheet, DateTime.Now, _functions, _instructionSetRepository, _referenceDataRepository);
            foreach(var key in dataSheet.Keys.ToList())
            {
                dataSheet[key] = 1;
            }

            //Convert back
            uiInputs.WalkNodes(PreOrder: jToken =>
            {
                switch (jToken)
                {
                    case JObject jObject:
                        {
                            if (!jObject.Properties().Any(prop => prop.Name == "input"))
                            {
                                break;
                            }

                            bool isInput = jObject["input"].ToObject<bool>();
                            string name = jObject["variableName"].ToObject<string>().ToLower().Trim();

                            if(!isInput && dataSheet.TryGetValue($"{moduleName}.{name}", out object value))
                            {
                                jObject["inputValue"] = JToken.FromObject(value);
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
