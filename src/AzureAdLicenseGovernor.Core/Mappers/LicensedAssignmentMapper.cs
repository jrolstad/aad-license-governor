using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicensedAssignmentMapper
    {
        public Models.LicenseAssignment Map(Microsoft.Graph.AssignedLicense toMap)
        {
            return new Models.LicenseAssignment
            {
                ProductId = toMap?.SkuId.ToString(),
                DisabledServicePlans = toMap?.DisabledPlans?.Select(s => s.ToString())?.ToList()
            };
        }

        public Microsoft.Graph.AssignedLicense Map(Models.LicenseAssignment toMap)
        {
            return new Microsoft.Graph.AssignedLicense
            {
                SkuId = Guid.Parse(toMap?.ProductId),
                DisabledPlans = toMap?.DisabledServicePlans?.Select(Guid.Parse) ?? new List<Guid>()
            };
        }

        public Models.LicenseAssignment Map(Models.ProductAssignment toMap, IEnumerable<string> servicePlansForProduct)
        {
            var disabledServicePlans = servicePlansForProduct
                .Except(toMap.EnabledServicePlans,StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new Models.LicenseAssignment
            {
                ProductId = toMap.Id,
                DisabledServicePlans = disabledServicePlans
            };
        }


    }
}
