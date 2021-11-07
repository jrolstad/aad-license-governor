namespace AzureAdLicenseGovernor.Core.Configuration
{
    public static class ConfigurationNames
    {
        public static CosmosConfigurationNames Cosmos = new CosmosConfigurationNames();
        public static KeyVaultConfigurationNames KeyVault = new KeyVaultConfigurationNames();

        public class CosmosConfigurationNames
        {
            public string BaseUri = "Cosmos_BaseUri";
        }

        public class KeyVaultConfigurationNames
        {
            public string BaseUri = "KeyVault_BaseUri";
            public string ManagedIdentityClient = "KeyVault_ManagedIdentityClientId";
        }
    }
}
