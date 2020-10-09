using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleTemplateService
    {
        private readonly IModuleTemplateRepository _moduleTemplateRepository;
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IBusinessOpportunityLineItemRepository _businessOpportunityLineItemRepository;
        private readonly IFunctionRepository _functionRepository;

        public ModuleTemplateService(IModuleTemplateRepository moduleTemplateRepository)
        {
            _moduleTemplateRepository = moduleTemplateRepository;
        }

        public ModuleTemplateService(IModuleTemplateRepository moduleTemplateRepository, IFunctionRepository functionRepository)
            : this(moduleTemplateRepository)
        {
            _functionRepository = functionRepository;
        }

        public ModuleTemplateService(IModuleTemplateRepository moduleTemplateRepository, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, IEstimateRepository estimateRepository, IBusinessOpportunityLineItemRepository businessOpportunityLineItemRepository)
            : this(moduleTemplateRepository)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _estimateRepository = estimateRepository;
            _businessOpportunityLineItemRepository = businessOpportunityLineItemRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _moduleTemplateRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _moduleTemplateRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _moduleTemplateRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _moduleTemplateRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Import(string env)
        {
            string envSetting = $"{env}-url";
            string url = AppSettings.Get(envSetting);
            if (url == null)
            {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/moduleTemplates/?object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _moduleTemplateRepository.Reset(jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }

        public async Task<(object, string)> CreateVariableNameList(JObject document)
        {
            var estimateService = new EstimateService(_instructionSetRepository, _referenceDataRepository, _estimateRepository, _businessOpportunityLineItemRepository);
            var dict = new Dictionary<string, object>();
            string moduleTitle = document["moduleTitle"].ToString().ToLower();
            estimateService.ParseFromJson(document, dict, moduleTitle);
            return (GetDatasheetNames(dict, moduleTitle).Distinct(), "OK");
        }

        private IEnumerable<string> GetDatasheetNames(Dictionary<string,object> dict, string moduleTitle)
        {
            foreach (var pair in dict)
            {
                switch (pair.Value)
                {
                    case List<Dictionary<string, object>> nestedDictList:
                        {
                            foreach (var nestedDict in nestedDictList)
                            {
                                foreach (var name in GetDatasheetNames(nestedDict, moduleTitle).Distinct())
                                {
                                    yield return name;
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (pair.Key.StartsWith(moduleTitle))
                            {
                                yield return pair.Key;
                            }
                        }
                        break;
                }
            }
        }

        public async Task<(object, string)> GetVariableNames(string moduleName, IDictionary<string, string> queryParams)
        {
            var set = new SortedSet<string>();
            {
                var moduleTemplateJson = await _moduleTemplateRepository.Get(moduleName, queryParams);
                if (moduleTemplateJson != null && queryParams.ContainsKey("id"))
                {
                    var moduleTemplate = JObject.Parse(moduleTemplateJson.ToString()).ToObject<ModuleTemplate>();
                    if (moduleTemplate.Rest != null && moduleTemplate.Rest.TryGetValue("variableNames", out object value))
                    {
                        IEnumerable<string> strings = null;
                        switch(value)
                        {
                            case IEnumerable<string> s:
                                strings = s;
                                break;
                            case JArray jArray:
                                strings = jArray.Select(jToken => jToken.ToString());
                                break;
                        }

                        foreach(string s in strings)
                        {
                            if(!set.Contains(s))
                            {
                                set.Add(s);
                            }
                        }
                    }
                }
            }
            {
                var functions = (List<Function>)await _functionRepository.List(new Dictionary<string, string>() { ["module"] = moduleName.ToLower(), ["object"] = "full" });

                foreach (string s in functions.Select(f => $"{f.Module}.{f.Name}".ToLower()))
                {
                    if (!set.Contains(s))
                    {
                        set.Add(s);
                    }
                }
            }
            return (set.ToList(), "OK");
        }
    }
}
