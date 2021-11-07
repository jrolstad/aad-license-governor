using AzureAdLicenseGovernor.Core.Models;
using Microsoft.Graph;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicensedProductMapper
    {
        public Product Map(SubscribedSku toMap)
        {
            return new Product
            {
                Id = toMap?.SkuId?.ToString(),
                Name = toMap?.SkuPartNumber,
                ServicePlans = toMap?.ServicePlans?.Select(Map)?.ToList()
            };
        }

        public ServicePlan Map(ServicePlanInfo toMap)
        {
            return new ServicePlan
            {
                Id = toMap?.ServicePlanId?.ToString(),
                Name = toMap?.ServicePlanName
            };
        }
    }
}
