using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class LicensedGroup
    {
        public string ObjectId { get; set; }
        public string TenantId { get; set; }

        public List<ProductAssignment> LicensedProducts { get; set; }

    }


}
