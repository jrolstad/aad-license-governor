using AzureAdLicenseGovernor.Core.Models;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class ProductComparer
    {
        public TenantProductComparisonResult Compare(ICollection<Product> actual,ICollection<Product> expected)
        {
            return new TenantProductComparisonResult();
        }
    }

}
