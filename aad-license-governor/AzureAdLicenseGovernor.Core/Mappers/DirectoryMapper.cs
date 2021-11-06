using AzureAdLicenseGovernor.Core.Configuration;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class DirectoryMapper
    {
        public Directory Map(DirectoryData toMap)
        {
            return new Directory
            {
                Id = toMap?.Id,
                TenantId = toMap?.TenantId,
                Name = toMap?.Name,
                Domain = toMap?.Domain,
                IsDefault = toMap?.IsDefault ?? false,
                CanManageObjects = toMap?.CanManageObjects ?? false,
                GraphUrl = toMap?.GraphUrl,
                PortalUrl = toMap?.PortalUrl,
                ClientId = toMap?.ClientId,
                ClientType = toMap?.ClientType ?? DirectoryClientType.Application
            };
        }

        public DirectoryData Map(Directory toMap)
        {
            return new DirectoryData
            {
                Id = toMap?.Id,
                TenantId = toMap?.TenantId,
                Area = CosmosConfiguration.DefaultPartitionKey,
                Name = toMap?.Name,
                Domain = toMap?.Domain,
                IsDefault = toMap?.IsDefault ?? false,
                CanManageObjects = toMap?.CanManageObjects ?? false,
                GraphUrl = toMap?.GraphUrl,
                PortalUrl = toMap?.PortalUrl,
                ClientId = toMap?.ClientId,
                ClientType = toMap?.ClientType ?? DirectoryClientType.Application
            };
        }
    }
}
