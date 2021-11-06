using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class ProductAssignment
    {
        public string Id { get; set; }

        public List<string> EnabledServicePlans { get; set; }
    }


}
