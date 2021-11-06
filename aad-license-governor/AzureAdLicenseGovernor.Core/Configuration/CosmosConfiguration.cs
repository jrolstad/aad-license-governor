using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Configuration
{
    public static class CosmosConfiguration
    {
        public static string DefaultPartitionKey = "Default";
        public static string DatabaseId = "aad-licensing-governor";
        public static CosmosContainers Containers = new CosmosContainers();
    }

    public class CosmosContainers
    {
        public string Directories = "Directories";
        public string LicensedGroups = "LicensedGroups";
    }
}
