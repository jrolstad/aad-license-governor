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
                AppliesTo = toMap?.AppliesTo,
                CapabilityStatus = toMap?.CapabilityStatus,
                ServicePlans = toMap?.ServicePlans?.Select(Map)?.ToList(),
                Units = new ProductUnits
                {
                    Consumed = toMap?.ConsumedUnits ?? 0,
                    Enabled = toMap?.PrepaidUnits?.Enabled ?? 0,
                    Suspended = toMap?.PrepaidUnits?.Suspended ?? 0,
                    Warning = toMap?.PrepaidUnits?.Warning ?? 0,
                }
            };
        }

        public ServicePlan Map(ServicePlanInfo toMap)
        {
            return new ServicePlan
            {
                Id = toMap?.ServicePlanId?.ToString(),
                Name = toMap?.ServicePlanName,
                AppliesTo =toMap?.AppliesTo,
                ProvisioningStatus = toMap?.ProvisioningStatus
            };
        }
    }
}
