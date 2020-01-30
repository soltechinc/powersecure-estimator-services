using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public partial class HealthService
    {
        public async Task<(object,string)> CheckProperties()
        {
            var missingProperties = new List<string>(MissingPropertiesIterator());

            return (missingProperties, $"{missingProperties.Count} properties missing");
        }
    }
}
