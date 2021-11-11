using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class ProductMapper
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

        public ICollection<Product> Map(ProductDataSnapshot snapshot)
        {
            return snapshot?
                .Products?
                .Select(Map)?
                .ToList() ?? new List<Product>();
        }

        public Product Map(ProductData toMap)
        {
            return new Product
            {
                Id = toMap?.Id,
                Name = toMap?.Name,
                AppliesTo = toMap?.AppliesTo,
                CapabilityStatus = toMap?.CapabilityStatus,
                ServicePlans = toMap?.ServicePlans,
                Units = toMap?.Units
            };
        }

        public ProductData Map(Product toMap)
        {
            return new ProductData
            {
                Id = toMap?.Id,
                Name = toMap?.Name,
                AppliesTo = toMap?.AppliesTo,
                CapabilityStatus = toMap?.CapabilityStatus,
                ServicePlans = toMap?.ServicePlans,
                Units = toMap?.Units
            };
        }

        public ProductDataSnapshot Map(string tenantId, ICollection<Product> data)
        {
            return new ProductDataSnapshot
            {
                Id = tenantId,
                TenantId = tenantId,
                Area = CosmosConfiguration.DefaultPartitionKey,
                SnapshotTakenAt = ClockService.Now,
                Products = data.Select(Map).ToList()
            };
        }
    }
}
