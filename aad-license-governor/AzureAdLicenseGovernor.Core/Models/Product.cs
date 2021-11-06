using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<ServicePlan> ServicePlans { get; set; }
    }
}
