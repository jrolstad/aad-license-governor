using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class UserMapper
    {
        public Models.User Map(Models.Directory directory, Microsoft.Graph.DirectoryObject toMap)
        {
            if (toMap == null) return null;

            if (toMap is Microsoft.Graph.User user)
            {
                return new Models.User
                {
                    TenantId = directory.TenantId,
                    ObjectId = user?.Id,
                    UserPrincipalName = user?.UserPrincipalName,
                    LicenseStates = user?.LicenseAssignmentStates?.Select(Map)?.ToList() ?? new List<Models.LicenseAssignmentState>()
                };
            }

            return new Models.User
            {
                TenantId = directory.TenantId,
                ObjectId = toMap?.Id,
                UserPrincipalName = null,
                LicenseStates = new List<Models.LicenseAssignmentState>()
            };
        }

        private Models.LicenseAssignmentState Map(Microsoft.Graph.LicenseAssignmentState toMap)
        {
            return new Models.LicenseAssignmentState
            {
                SkuId = toMap?.SkuId?.ToString(),
                Status = toMap?.State,
                Error = toMap?.Error
            };
        }
    }
}
