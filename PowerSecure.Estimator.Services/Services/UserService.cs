using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(object, string)> List(IDictionary<string, string> queryParams)
        {
            return (await _userRepository.List(queryParams), "OK");
        }

        public async Task<(object, string)> Get(string id, IDictionary<string, string> queryParams)
        {
            return (await _userRepository.Get(id), "OK");
        }

        public async Task<(object, string)> Upsert(JObject document)
        {
            return (await _userRepository.Upsert(document), "OK");
        }

        public async Task<(object, string)> Delete(string id, IDictionary<string, string> queryParams)
        {
            int deletedDocumentCount = await _userRepository.Delete(id);
            return (deletedDocumentCount, $"{deletedDocumentCount} documents deleted");
        }
    }
}
