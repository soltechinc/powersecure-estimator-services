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
using System.Net.Http;
using PowerSecure.Estimator.Services.Endpoints;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using PowerSecure.Estimator.Services.Models;
using Microsoft.Azure.Documents;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Packaging;
using System.Xml;
using System.IO;
using System.Xml.Xsl;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateTemplateService
    {
        private readonly IEstimateTemplateRepository _estimateTemplateRepository;

        public EstimateTemplateService(IEstimateTemplateRepository estimateTemplateRepository) {
            _estimateTemplateRepository = estimateTemplateRepository;
        }
        
        public async Task<(object, string)> List(IDictionary<string, string> queryParams) {
            return (await _estimateTemplateRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams) {
            return (await _estimateTemplateRepository.Get(id, queryParams), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document) {
            return (await _estimateTemplateRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams) {
            int deletedDocumentCount = await _estimateTemplateRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
