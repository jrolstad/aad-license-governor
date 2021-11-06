using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class GroupMapper
    {
        public Models.Group Map(Models.Directory directory, Microsoft.Graph.Group toMap)
        {
            return new Models.Group
            {
                ObjectId = toMap?.Id,
                TenantId = directory.TenantId,
                DisplayName = toMap?.DisplayName,
                AssignedLicenses = toMap?.AssignedLicenses?.Select(Map)?.ToList()
            };
        }

        private Models.LicenseAssignment Map(Microsoft.Graph.AssignedLicense toMap)
        {
            return new Models.LicenseAssignment
            {
                ProductId = toMap?.SkuId.ToString(),
                DisabledServicePlans = toMap?.DisabledPlans?.Select(s=>s.ToString())?.ToList()
            };
        }
    }
}
