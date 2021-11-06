using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class Group
    {
        public string TenantId { get; set; }
        public string ObjectId { get; set; }
        public string DisplayName { get; set; }

        public List<LicenseAssignment> AssignedLicenses { get; set; }
    }

    public class LicenseAssignment
    {
        public string ProductId { get; set; }
        public List<string> DisabledServicePlans { get; set; }
    }
}
