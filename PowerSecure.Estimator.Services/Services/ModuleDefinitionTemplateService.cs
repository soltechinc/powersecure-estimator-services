using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleDefinitionTemplateService
    {
        private readonly IEstimateTemplateRepository _estimateTemplateRepository;
        private readonly ILogger _log;

        public ModuleDefinitionTemplateService(IEstimateTemplateRepository estimateTemplateRepository, ILogger log = null)
        {
            _estimateTemplateRepository = estimateTemplateRepository;
            _log = log ?? NullLogger.Instance;
        }

        public async Task<(object, string)> Get(string estimateTemplateId, string id, IDictionary<string, string> queryParams)
        {
            var estimateTemplateDoc = await _estimateTemplateRepository.Get(estimateTemplateId);

            if (estimateTemplateDoc == null)
            {
                return (null, "Estimate template id not found");
            }

            var estimateTemplate = JObject.Parse(estimateTemplateDoc.ToString()).ToObject<EstimateTemplate>();

            foreach (var moduleDefinitionTemplate in estimateTemplate.Modules)
            {
                if (moduleDefinitionTemplate.Id == id)
                {
                    return (moduleDefinitionTemplate, "OK");
                }
            }

            return (null, "Module definition template id not found");
        }

        public async Task<(object, string)> Upsert(string estimateTemplateId, JObject document)
        {
            var estimateTemplateDoc = await _estimateTemplateRepository.Get(estimateTemplateId);

            if (estimateTemplateDoc == null)
            {
                return (null, "Estimate template id not found");
            }

            var estimateTemplate = JObject.Parse(estimateTemplateDoc.ToString()).ToObject<EstimateTemplate>();

            var newModuleDefinitionTemplate = document.ToObject<ModuleDefinition>();

            if (string.IsNullOrEmpty(newModuleDefinitionTemplate.Id))
            {
                newModuleDefinitionTemplate.Id = Guid.NewGuid().ToString();
                estimateTemplate.Modules.Add(newModuleDefinitionTemplate);
            }
            else
            {
                bool added = false;
                for (int i = 0; i < estimateTemplate.Modules.Count; ++i)
                {
                    if (estimateTemplate.Modules[i].Id == newModuleDefinitionTemplate.Id)
                    {
                        estimateTemplate.Modules[i] = newModuleDefinitionTemplate;
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    return (null, "Module definition template id not found");
                }
            }

            await _estimateTemplateRepository.Upsert(JObject.FromObject(estimateTemplate));

            return (newModuleDefinitionTemplate, "OK");
        }

        public async Task<(object, string)> Delete(string estimateTemplateId, string id, IDictionary<string, string> queryParams)
        {
            var estimateTemplateDoc = await _estimateTemplateRepository.Get(estimateTemplateId);

            if (estimateTemplateDoc == null)
            {
                return (null, "Estimate template id not found");
            }

            var estimateTemplate = JObject.Parse(estimateTemplateDoc.ToString()).ToObject<EstimateTemplate>();

            bool deleted = false;
            for (int i = 0; i < estimateTemplate.Modules.Count; ++i)
            {
                if (estimateTemplate.Modules[i].Id == id)
                {
                    estimateTemplate.Modules.RemoveAt(i);
                    deleted = true;
                    break;
                }
            }

            if (deleted)
            {
                await _estimateTemplateRepository.Upsert(JObject.FromObject(estimateTemplate));
            }

            int deletedDocumentCount = deleted ? 1 : 0;
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
