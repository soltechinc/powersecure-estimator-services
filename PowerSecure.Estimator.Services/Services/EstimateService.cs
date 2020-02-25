using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class EstimateService
    {
        private readonly IFunctionRepository _functionRepository;

        public EstimateService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<(object, string)> Evaluate(JObject uiInputs)
        {
            //TODO - Add functionality
            return (null, "");
        }
    }
}
