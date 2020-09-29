using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class ModuleTemplateService
    {
        private readonly IModuleTemplateRepository _moduleTemplateRepository;

        public ModuleTemplateService(IModuleTemplateRepository moduleTemplateRepository)
        {
            _moduleTemplateRepository = moduleTemplateRepository;
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
    }
}
