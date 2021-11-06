using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models.Data
{
    public class LicensedGroupData
    {
        public string Id { get; set; }
        public string ObjectId { get; set; }
        public string TenantId { get; set; }
        public string Area { get; set; }
        public List<LicensedProductAssignmentData> LicensedProducts { get; set; }
    }

    public class LicensedProductAssignmentData
    {
        public string Id { get; set; }

        public List<string> EnabledServicePlans { get; set; }
    }
}
