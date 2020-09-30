using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Models;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class PermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;

        public PermissionsService(IPermissionsRepository permissionsRepository)
        {
            _permissionsRepository = permissionsRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            var permissions = (List<Permissions>)await _permissionsRepository.List(queryParams);
            if(queryParams.TryGetValue("object",out string value) && value.ToLower() == "full")
            {
                return (permissions, "OK");
            }

            return (permissions.Select(x => x.Permission).ToArray(), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _permissionsRepository.Get(id), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _permissionsRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _permissionsRepository.Delete(id);
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

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/security/permissions/?object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _permissionsRepository.Reset(jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }
    }
}
