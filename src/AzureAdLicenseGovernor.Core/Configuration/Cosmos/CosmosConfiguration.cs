namespace AzureAdLicenseGovernor.Core.Configuration.Cosmos
{
    public static class CosmosConfiguration
    {
        public static string DefaultPartitionKey = "Default";
        public static string DatabaseId = "aadlicense";
        public static CosmosContainers Containers = new CosmosContainers();
    }
}

