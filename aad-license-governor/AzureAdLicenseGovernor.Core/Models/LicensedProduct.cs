using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class LicensedProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<ServicePlan> ServicePlans { get; set; }
    }

    public class ServicePlan
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
