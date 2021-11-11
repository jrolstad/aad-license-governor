using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
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
                Mode = toMap?.Mode ?? ProductAssignmentMode.Audit,
                LicensedProducts = toMap?.LicensedProducts?.Select(Map)?.ToList(),
                TrackLicenseProcessingState = toMap?.TrackLicenseProcessingState ?? false
            };
        }

        private ProductAssignment Map(LicensedProductAssignmentData toMap)
        {
            return new ProductAssignment
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
                ObjectId = toMap?.ObjectId,
                TenantId = toMap?.TenantId,
                Mode = toMap?.Mode ?? ProductAssignmentMode.Audit,
                LicensedProducts = toMap?.LicensedProducts?.Select(Map)?.ToList(),
                TrackLicenseProcessingState = toMap?.TrackLicenseProcessingState ?? false
            };
        }

        public string GetId(string tenantId, string objectId)
        {
            return $"{tenantId}|{objectId}";
        }

        private LicensedProductAssignmentData Map(ProductAssignment toMap)
        {
            return new LicensedProductAssignmentData
            {
                Id = toMap?.Id,
                EnabledServicePlans = toMap?.EnabledServicePlans
            };
        }
    }
}
