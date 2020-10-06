using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateTemplateService
    {
        private readonly IEstimateTemplateRepository _estimateTemplateRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IModuleDefinitionRepository _moduleDefinitionRepository;
        private readonly IInstructionSetRepository _instructionSetRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IBusinessOpportunityLineItemRepository _businessOpportunityLineItemRepository;
        private readonly ILogger _log;

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository, ILogger log = null)
        {
            _estimateTemplateRepository = estimateTemplateRepository;
            _log = log;
        }

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository, IEstimateRepository estimateRepository, IModuleDefinitionRepository moduleDefinitionRepository, IInstructionSetRepository instructionSetRepository, IReferenceDataRepository referenceDataRepository, IBusinessOpportunityLineItemRepository businessOpportunityLineItemRepository, ILogger log)
            : this(estimateTemplateRepository, log)
        {
            _estimateRepository = estimateRepository;
            _moduleDefinitionRepository = moduleDefinitionRepository;
            _instructionSetRepository = instructionSetRepository;
            _referenceDataRepository = referenceDataRepository;
            _businessOpportunityLineItemRepository = businessOpportunityLineItemRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _estimateTemplateRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _estimateTemplateRepository.Get(id), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _estimateTemplateRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _estimateTemplateRepository.Delete(id);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        public async Task<(object, string)> Convert(string id, JObject document)
        {
            var inputEstimate = document.ToObject<Estimate>();

            var estimateTemplateJObject = JObject.Parse((await Get(id, null)).Item1.ToString());
            {
                var estimateTemplateProjection = estimateTemplateJObject.ToObject<Estimate>();
                inputEstimate.ProjectType = estimateTemplateProjection.ProjectType;
                inputEstimate.Market = estimateTemplateProjection.Market;
                inputEstimate.OutsideEquipmentPercent = estimateTemplateProjection.OutsideEquipmentPercent;
                inputEstimate.SoftCostPercent = estimateTemplateProjection.SoftCostPercent;
                inputEstimate.DesiredRateForInstall = estimateTemplateProjection.DesiredRateForInstall;
            }

            var outputEstimate = JObject.Parse((await new EstimateService(_estimateRepository).Upsert(JObject.FromObject(inputEstimate, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }))).Item1.ToString()).ToObject<Estimate>();

            _log.LogInformation("Created estimate - id " + outputEstimate.Id);

            var estimateTemplate = estimateTemplateJObject.ToObject<EstimateTemplate>();

            {
                var moduleDefinitionService = new ModuleDefinitionService(_moduleDefinitionRepository, _instructionSetRepository, _referenceDataRepository, _estimateRepository, _businessOpportunityLineItemRepository, _log);
                foreach (var moduleDefinition in estimateTemplate.Modules)
                {
                    moduleDefinition.Id = null;
                    moduleDefinition.ModuleId = null;
                    moduleDefinition.Rest.Add("boliNumber", outputEstimate.BOLINumber);
                    moduleDefinition.Rest.Add("estimateId", outputEstimate.Id);

                    //_log.LogInformation("New module definition - " + JObject.FromObject(moduleDefinition, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }).ToString());

                    var outputModuleDefinition = JObject.Parse((await moduleDefinitionService.Upsert(JObject.FromObject(moduleDefinition, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }))).Item1.ToString()).ToObject<ModuleDefinition>();

                    _log.LogInformation("Created module definition - id " + outputModuleDefinition.Id);
                }
            }

            return (await _estimateRepository.Get(outputEstimate.BOLINumber, new Dictionary<string, string> { ["id"] = outputEstimate.Id }), "OK");
        }
    }
}
