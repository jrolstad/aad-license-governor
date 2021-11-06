using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class LicensedGroup
    {
        public string ObjectId { get; set; }
        public string TenantId { get; set; }

        public List<LicensedProduct> LicensedProducts { get; set; }

    }

    public class LicensedProduct
    {
        public string Id { get; set; }

        public List<string> EnabledServicePlans { get; set; }
    }


}
