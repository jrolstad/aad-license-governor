using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class LicenseAssignment
    {
        public string ProductId { get; set; }
        public List<string> DisabledServicePlans { get; set; }
    }
}
