using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleDefinitionService
    {
        private readonly IModuleDefinitionRepository _moduleDefinitionRepository;
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly ILogger _log;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IBusinessOpportunityLineItemRepository _businessOpportunityLineItemRepository;

        public ModuleDefinitionService(IModuleDefinitionRepository moduleDefinitionRepository, IEstimateRepository estimateRepository = null, ILogger log = null)
        {
            _moduleDefinitionRepository = moduleDefinitionRepository;
            _estimateRepository = estimateRepository;
            _log = log ?? NullLogger.Instance;
        }

        public ModuleDefinitionService(IModuleDefinitionRepository moduleDefinitionRepository, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, IEstimateRepository estimateRepository, IBusinessOpportunityLineItemRepository businessOpportunityLineItemRepository, ILogger log) : this(moduleDefinitionRepository)
        {
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _estimateRepository = estimateRepository;
            _businessOpportunityLineItemRepository = businessOpportunityLineItemRepository;
            _log = log;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _moduleDefinitionRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _moduleDefinitionRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            if ((!document.Properties().Any(x => x.Name == "moduleId")) || string.IsNullOrWhiteSpace(document["moduleId"].ToString()))
            {
                document["moduleId"] = Guid.NewGuid().ToString();
            }
            {
                if (document.Properties().Any(x => x.Name == "datacache"))
                {
                    document.Remove("datacache");
                }
                string moduleTitle = document["moduleTitle"].ToString().ToLower();
                DateTime effectiveDate = document.Properties().Any(x => x.Name == "effectiveDate") ? DateTime.Parse(document["effectiveDate"].ToString()) : DateTime.Now;
                string cachedInstructionSets = (string)_referenceDataRepository.Lookup("factor", new List<(string, string)> { ("module", moduleTitle) }.ToArray(), effectiveDate, "instructionsetcache");

                var instructionSetNames = cachedInstructionSets == null ? new string[0] : cachedInstructionSets.Split(',').Select(x => $"{moduleTitle}.{x.ToLower().Trim()}");
                var dataSheet = new Dictionary<string, object>
                {
                    { $"{moduleTitle}.estimatematerialcost", null },
                    { $"{moduleTitle}.estimatelaborcost", null },
                    { $"{moduleTitle}.estimatetotalcost", null },
                    { $"{moduleTitle}.estimatematerialusetax", null },
                    { $"{moduleTitle}.estimatecostwithusetax", null },
                    { $"{moduleTitle}.estimatesellprice", null }
                };
                foreach (var instructionSetName in instructionSetNames)
                {
                    dataSheet.Add(instructionSetName, null);
                }
                var keysToEvaluate = dataSheet.Keys.ToArray();
                {
                    var estimateService = new EstimateService(_instructionSetRepository, _referenceDataRepository, _estimateRepository, _businessOpportunityLineItemRepository);
                    estimateService.IncludeEstimateData(document, dataSheet, moduleTitle);
                    estimateService.ParseFromJson(document, dataSheet, moduleTitle);
                }
                dataSheet.Add("all.effectivedate", effectiveDate.ToString("M/d/yyyy"));
                var resultDataSheet = new RulesEngine().EvaluateDataSheet(dataSheet, keysToEvaluate, effectiveDate, Primitive.Load(), _instructionSetRepository, _referenceDataRepository, _log, new HashSet<string>());
                var dataCacheDict = new Dictionary<string, object>();
                foreach (var instructionSetName in instructionSetNames)
                {
                    if (resultDataSheet.ContainsKey(instructionSetName) && resultDataSheet[instructionSetName] != null)
                    {
                        dataCacheDict.Add(instructionSetName, resultDataSheet[instructionSetName]);
                    }
                }
                if (dataCacheDict.Count > 0)
                {
                    document.Add("datacache", JToken.FromObject(dataCacheDict));
                }

                document.UpdateKeyWithValue("materialCost", dataSheet[$"{moduleTitle}.estimatematerialcost"]);
                document.UpdateKeyWithValue("laborCost", dataSheet[$"{moduleTitle}.estimatelaborcost"]);
                document.UpdateKeyWithValue("totalCost", dataSheet[$"{moduleTitle}.estimatetotalcost"]);
                document.UpdateKeyWithValue("materialUseTax", dataSheet[$"{moduleTitle}.estimatematerialusetax"]);
                document.UpdateKeyWithValue("totalCostWithTax", dataSheet[$"{moduleTitle}.estimatecostwithusetax"]);
                document.UpdateKeyWithValue("sellPrice", dataSheet[$"{moduleTitle}.estimatesellprice"]);
            }

            var retValue = (await _moduleDefinitionRepository.Upsert(document), "OK");
            var moduleDefinitionDocument = JObject.Parse(retValue.Item1.ToString());

            {
                string boliNumber = moduleDefinitionDocument.Properties().Any(x => x.Name == "boliNumber") ? moduleDefinitionDocument["boliNumber"].ToString() : null;
                string estimateId = moduleDefinitionDocument.Properties().Any(x => x.Name == "estimateId") ? moduleDefinitionDocument["estimateId"].ToString() : null;
                string moduleId = moduleDefinitionDocument.Properties().Any(x => x.Name == "moduleId") ? moduleDefinitionDocument["moduleId"].ToString() : null;

                if (boliNumber != null && estimateId != null && moduleId != null)
                {
                    bool found = false;
                    Estimate estimate = JObject.Parse(((Document)_estimateRepository.Get(boliNumber, new Dictionary<string, string> { ["id"] = estimateId }).GetAwaiter().GetResult()).ToString()).ToObject<Estimate>();

                    if (estimate.Modules == null)
                    {
                        estimate.Modules = new List<ModuleDefinition>();
                    }

                    for (int i = 0; i < estimate.Modules.Count; ++i)
                    {
                        if (estimate.Modules[i].ModuleId == moduleId)
                        {
                            estimate.Modules[i] = moduleDefinitionDocument.ToObject<ModuleDefinition>();
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        estimate.Modules.Add(moduleDefinitionDocument.ToObject<ModuleDefinition>());
                    }

                    await _estimateRepository.Upsert(JObject.FromObject(estimate));
                }
            }
            return retValue;
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            {
                var moduleDefinitionTuple = await Get(id, queryParams);
                var document = JObject.FromObject(moduleDefinitionTuple.Item1);

                string boliNumber = document.Properties().Any(x => x.Name == "boliNumber") ? document["boliNumber"].ToString() : null;
                string estimateId = document.Properties().Any(x => x.Name == "estimateId") ? document["estimateId"].ToString() : null;
                string moduleId = document.Properties().Any(x => x.Name == "moduleId") ? document["moduleId"].ToString() : null;

                if (boliNumber != null && estimateId != null && moduleId != null)
                {
                    Estimate estimate = JObject.Parse(((Document)_estimateRepository.Get(boliNumber, new Dictionary<string, string> { ["id"] = estimateId }).GetAwaiter().GetResult()).ToString()).ToObject<Estimate>();

                    if (estimate.Modules != null)
                    {
                        for (int i = 0; i < estimate.Modules.Count; ++i)
                        {
                            if (estimate.Modules[i].ModuleId == moduleId)
                            {
                                estimate.Modules.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    await _estimateRepository.Upsert(JObject.FromObject(estimate));
                }
            }

            int deletedDocumentCount = await _moduleDefinitionRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
