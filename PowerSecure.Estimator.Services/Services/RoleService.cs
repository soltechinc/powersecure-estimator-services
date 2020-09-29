using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _roleRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _roleRepository.Get(id), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _roleRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _roleRepository.Delete(id);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
