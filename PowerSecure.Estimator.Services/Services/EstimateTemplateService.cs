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

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository, IEstimateRepository estimateRepository = null, IModuleDefinitionRepository moduleDefinitionRepository = null)
        {
            _estimateTemplateRepository = estimateTemplateRepository;
            _estimateRepository = estimateRepository;
            _moduleDefinitionRepository = moduleDefinitionRepository;
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

        public async Task<(object, string)> Convert(string id, JObject document, IDictionary<string, string> queryParams)
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

            var outputEstimate = JObject.Parse((await new EstimateService(_estimateRepository).Upsert(JObject.FromObject(inputEstimate))).Item1.ToString()).ToObject<Estimate>();

            var estimateTemplate = estimateTemplateJObject.ToObject<EstimateTemplate>();

            {
                var moduleDefinitionService = new ModuleDefinitionService(_moduleDefinitionRepository);
                foreach (var moduleDefinition in estimateTemplate.Modules)
                {
                    moduleDefinition.Id = null;
                    moduleDefinition.ModuleId = null;
                    moduleDefinition.Rest.Add("boliNumber", outputEstimate.BOLINumber);
                    moduleDefinition.Rest.Add("estimateId", outputEstimate.Id);
                    await moduleDefinitionService.Upsert(JObject.FromObject(moduleDefinition));
                }
            }

            return (await _estimateRepository.Get(outputEstimate.BOLINumber, new Dictionary<string, string> { ["id"] = outputEstimate.Id }), "OK");
        }
    }
}
