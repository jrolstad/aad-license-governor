using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Configuration.Cosmos
{
    public static class CosmosConfiguration
    {
        public static string DefaultPartitionKey = "Default";
        public static string DatabaseId = "aad-licensing-governor";
        public static CosmosContainers Containers = new CosmosContainers();
    }
}
