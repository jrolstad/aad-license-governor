using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class TenantProductComparisonResult
    {
        public ICollection<Product> Added { get; set; }
        public ICollection<Product> Removed { get; set; }
        public ICollection<ProductComparisonResult> Updated { get; set; }
    }

    public class ProductComparisonResult
    {
       public Product Product { get; set; }
       public ServicePlanComparisonResult ServicePlans { get; set; }
    }

    public class ServicePlanComparisonResult
    {
        public ICollection<ServicePlan> Added { get; set; }
        public ICollection<ServicePlan> Removed { get; set; }
    }
}
