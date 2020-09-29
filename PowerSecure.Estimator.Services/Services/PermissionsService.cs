using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
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
            return (await _permissionsRepository.List(queryParams), "OK");
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
    }
}
