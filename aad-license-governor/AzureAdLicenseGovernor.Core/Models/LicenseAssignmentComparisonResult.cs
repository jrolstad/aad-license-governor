using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class LicenseAssignmentComparisonResult
    {
        public List<LicenseAssignment> ToAdd { get; set; }
        public List<LicenseAssignment> ToRemove { get; set; }
        public List<LicenseAssignment> ToUpdate { get; set; }
    }
}
