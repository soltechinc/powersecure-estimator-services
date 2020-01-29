using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<object> List(IDictionary<string,string> queryParams)
        {
            return await _factorRepository.List(queryParams);
        }

        public async Task<object> Get(string id, IDictionary<string, string> queryParams)
        {
            return await _factorRepository.Get(id, queryParams);
        }

        public async Task<object> Upsert(JObject document)
        {
            document["key"] = string.Join('-', string.Empty, document["module"], document["returnAttribute"]);
            document["hash"] = CreateHash(document.Properties()
                                .Where(o => o.Name != "id" && o.Name != "hash" && !o.Name.StartsWith("_"))
                                .SelectMany(o => new string[] { o.Name, o.Value.ToString() })
                                .OrderBy(s => s)
                                .Aggregate(new StringBuilder(), (sb, s) => sb.AppendFormat("-{0}", s)).ToString());
            return await _factorRepository.Upsert(document);
        }

        public async Task<object> Delete(string id, IDictionary<string, string> queryParams)
        {
            return await _factorRepository.Delete(id, queryParams);
        }

        private static string CreateHash(string valueKey)
        {
            return MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(valueKey))
                      .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
        }
    }
}
