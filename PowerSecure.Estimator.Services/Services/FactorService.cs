using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class FactorService
    {
        private readonly IFactorRepository _factorRepository;

        public FactorService(IFactorRepository factorRepository)
        {
            _factorRepository = factorRepository;
        }

        public async Task<(object, string)> List(IDictionary<string,string> queryParams)
        {
            return (await _factorRepository.List(queryParams),"OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _factorRepository.Get(id, queryParams),"OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            document["key"] = string.Join('-', string.Empty, document["module"], document["returnattribute"]);
            document["hash"] = CreateHash(document.Properties()
                                .Where(o => o.Name != "id" && o.Name != "hash" && !o.Name.StartsWith("_"))
                                .SelectMany(o => new string[] { o.Name, o.Value.ToString() })
                                .OrderBy(s => s)
                                .Aggregate(new StringBuilder(), (sb, s) => sb.AppendFormat("-{0}", s)).ToString());
            if(!document.ContainsKey("creationdate"))
            {
                document.Add("creationdate", JToken.FromObject(DateTime.Now.ToString("M/d/yyyy")));
            }
            return (await _factorRepository.Upsert(document),"OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _factorRepository.Delete(id, queryParams);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }

        private static string CreateHash(string valueKey)
        {
            return MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(valueKey))
                      .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<(object, string)> Import(string env, string module)
        {
            string envSetting = $"{env}-url";
            string url = Environment.GetEnvironmentVariable(envSetting);
            if(url == null)
            {
                return (null, $"Unable to find environment setting: {envSetting}");
            }

            string returnValue = await _httpClient.GetStringAsync($"{url}/api/factors/?module={module}&object=full");
            var jObj = JObject.Parse(returnValue);

            if(jObj["Status"].ToString() != "200")
            {
                return (null, "Error when calling list api");
            }

            int newDocumentCount = await _factorRepository.Reset(module, jObj["Items"]);

            return (newDocumentCount, $"{newDocumentCount} documents created.");
        }

        public async Task<(object, string)> ListStates()
        {
            IDictionary<string, string> queryParams = new Dictionary<string, string>() { { "module", "all" }, { "returnattribute", "usstate" } };

            return (await _factorRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> ListVerticalMarkets()
        {
            IDictionary<string, string> queryParams = new Dictionary<string, string>() { { "module", "all" }, { "returnattribute", "verticalmarket" } };

            return (await _factorRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> ListProjectTypes()
        {
            IDictionary<string, string> queryParams = new Dictionary<string, string>() { { "module", "all" }, { "returnattribute", "projecttype" } };

            return (await _factorRepository.List(queryParams), "OK");
        }
    }
}
