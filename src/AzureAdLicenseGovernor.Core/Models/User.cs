using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class User
    {
        public string TenantId { get; set; }
        public string ObjectId { get; set; }
        public string UserPrincipalName { get; set; }
        public ICollection<LicenseAssignmentState> LicenseStates { get; set; }
    }

    public class LicenseAssignmentState
    {
        public string SkuId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
