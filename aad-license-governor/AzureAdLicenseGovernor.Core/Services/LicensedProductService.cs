using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class LicensedProductService
    {
        public Task<List<LicensedProduct>> Get()
        {
            return Task.FromResult(new List<LicensedProduct>());
        }
    }
}
