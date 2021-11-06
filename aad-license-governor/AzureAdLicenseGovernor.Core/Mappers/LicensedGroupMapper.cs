using AzureAdLicenseGovernor.Core.Configuration;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicensedGroupMapper
    {
        public LicensedGroup Map(LicensedGroupData toMap)
        {
            return new LicensedGroup
            {
                ObjectId = toMap?.ObjectId,
                TenantId = toMap?.TenantId,
                LicensedProducts = toMap?.LicensedProducts?.Select(Map)?.ToList()
            };
        }

        private LicensedProduct Map(LicensedProductData toMap)
        {
            return new LicensedProduct
            {
                Id = toMap?.Id,
                EnabledServicePlans = toMap?.EnabledServicePlans
            };
        }

        public LicensedGroupData Map(LicensedGroup toMap)
        {
            return new LicensedGroupData
            {
                Id = GetId(toMap?.TenantId,toMap?.ObjectId),
                Area = CosmosConfiguration.DefaultPartitionKey,
                ObjectId = toMap?.ObjectId,
                TenantId = toMap?.TenantId,
                LicensedProducts = toMap?.LicensedProducts?.Select(Map)?.ToList()
            };
        }

        public string GetId(string tenantId, string objectId)
        {
            return $"{tenantId}|{objectId}";
        }

        private LicensedProductData Map(LicensedProduct toMap)
        {
            return new LicensedProductData
            {
                Id = toMap?.Id,
                EnabledServicePlans = toMap?.EnabledServicePlans
            };
        }
    }
}
