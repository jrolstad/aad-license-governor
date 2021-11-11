using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AppliesTo { get; set; }
        public string CapabilityStatus { get; set; }
        public ProductUnits Units {get;set; }
        public List<ServicePlan> ServicePlans { get; set; }
    }

    public class ProductUnits
    {
        public int Consumed { get; set; }
        public int Enabled { get; set; }
        public int Suspended { get; set; }
        public int Warning { get; set; }
    }
}
