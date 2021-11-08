using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using System;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class DataExtensions
    {
        public static DirectoryData WithDirectory(this TestBuilderBase root,
            string name,
            string tenantId = null,
            string domain = null,
            string graphUrl = "https://graph.microsoft.com",
            string portalUrl = "https://portal.azure.com",
            DirectoryClientType clientType = DirectoryClientType.Application,
            string clientId = "client-id",
            bool isDefault = true
        )
        {
            var data = new DirectoryData
            {
                Id = Guid.NewGuid().ToString(),
                TenantId = tenantId ?? Guid.NewGuid().ToString(),
                Area = CosmosConfiguration.DefaultPartitionKey,
                Name = name,
                Domain = domain ?? Guid.NewGuid().ToString(),
                GraphUrl = graphUrl,
                PortalUrl = portalUrl,
                ClientType = clientType,
                ClientId = clientId,
                IsDefault = isDefault

            };

            root.Context.Data.Directories.AddOrUpdate(data.Id, data, (id, existing) => { return data; });

            return data;
        }
    }
}
