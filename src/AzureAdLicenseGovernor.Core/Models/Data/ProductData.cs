using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models.Data
{

    public class ProductData
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string AppliesTo { get; set; }
        public string CapabilityStatus { get; set; }
        public ProductUnits Units { get; set; }
        public List<ServicePlan> ServicePlans { get; set; }
    }
}
