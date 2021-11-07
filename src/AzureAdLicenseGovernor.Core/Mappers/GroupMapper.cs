using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class GroupMapper
    {
        private readonly LicensedAssignmentMapper _licensedAssignmentMapper;

        public GroupMapper(LicensedAssignmentMapper licensedAssignmentMapper)
        {
            _licensedAssignmentMapper = licensedAssignmentMapper;
        }

        public Models.Group Map(Models.Directory directory, Microsoft.Graph.Group toMap)
        {
            return new Models.Group
            {
                ObjectId = toMap?.Id,
                TenantId = directory.TenantId,
                DisplayName = toMap?.DisplayName,
                AssignedLicenses = toMap?.AssignedLicenses?.Select(_licensedAssignmentMapper.Map)?.ToList()
            };
        }

        
    }
}
