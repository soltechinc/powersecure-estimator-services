using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly ILogger _log;

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository, IEstimateRepository estimateRepository = null, IModuleDefinitionRepository moduleDefinitionRepository = null, ILogger log = null)
        {
            _estimateTemplateRepository = estimateTemplateRepository;
            _estimateRepository = estimateRepository;
            _moduleDefinitionRepository = moduleDefinitionRepository;
            _log = log;
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

            _log.LogInformation("New estimate - " + JObject.FromObject(inputEstimate, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }).ToString());

            var outputEstimate = JObject.Parse((await new EstimateService(_estimateRepository).Upsert(JObject.FromObject(inputEstimate, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }))).Item1.ToString()).ToObject<Estimate>();

            _log.LogInformation("Created estimate - id " + outputEstimate.Id);

            var estimateTemplate = estimateTemplateJObject.ToObject<EstimateTemplate>();

            {
                var moduleDefinitionService = new ModuleDefinitionService(_moduleDefinitionRepository, _estimateRepository, _log);
                foreach (var moduleDefinition in estimateTemplate.Modules)
                {
                    moduleDefinition.Id = null;
                    moduleDefinition.ModuleId = null;
                    moduleDefinition.Rest.Add("boliNumber", outputEstimate.BOLINumber);
                    moduleDefinition.Rest.Add("estimateId", outputEstimate.Id);
                    var outputModuleDefinition = JObject.Parse((await moduleDefinitionService.Upsert(JObject.FromObject(moduleDefinition, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }))).Item1.ToString()).ToObject<ModuleDefinition>();

                    _log.LogInformation("Created module definition - id " + outputModuleDefinition.Id);
                }
            }

            return (await _estimateRepository.Get(outputEstimate.BOLINumber, new Dictionary<string, string> { ["id"] = outputEstimate.Id }), "OK");
        }
    }
}
