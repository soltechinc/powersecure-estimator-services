using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class FunctionService
    {
        private readonly IFunctionRepository _functionRepository;

        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.List(queryParams),"OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _functionRepository.Get(id, queryParams),"OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            if (!document.ContainsKey("creationdate"))
            {
                document.Add("creationdate", JToken.FromObject(DateTime.Now.ToString("M/d/yyyy")));
            }
            if(document.ContainsKey("module"))
            {
                document["module"] = document["module"].ToString().ToLower();
            }
            if (document.ContainsKey("name"))
            {
                document["name"] = document["name"].ToString().ToLower();
            }
            return (await _functionRepository.Upsert(document),"OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _functionRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Import(string env, string module)
        {
            string envSetting = $"{env}-url";
            string url = Environment.GetEnvironmentVariable(envSetting);
            if (url == null)
            {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/functions/?module={module}&object=full");
            var jObj = JObject.Parse(returnValue);

            if (jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _functionRepository.Reset(module, jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }
    }
}
