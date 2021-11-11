using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Orchestrators;
using AzureAdLicenseGovernor.Core.Repositories;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AzureAdLicenseGovernor.Core.Configuration.DependencyInjection
{
    public static class DependencyInjectionConfiguration
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IGraphClientFactory, GraphClientFactory>();

            services.AddTransient<DirectoryMapper>();
            services.AddTransient<GroupMapper>();
            services.AddTransient<LicenseAssignmentComparer>();
            services.AddTransient<LicensedAssignmentMapper>();
            services.AddTransient<LicensedGroupMapper>();
            services.AddTransient<ProductComparer>();
            services.AddTransient<ProductMapper>();

            services.AddTransient<DirectoryOrchestrator>();
            services.AddTransient<LicensedGroupGovernanceOrchestrator>();
            services.AddTransient<LicensedGroupMonitoringOrchestrator>();
            services.AddTransient<LicensedGroupOrchestrator>();
            services.AddTransient<ProductMonitoringOrchestrator>();

            services.AddTransient<DirectoryRepository>();
            services.AddTransient<LicensedGroupRepository>();
            services.AddTransient<ProductRepository>();

            services.AddTransient<AuthorizationService>();
            services.AddTransient<CosmosDbService>();
            services.AddTransient<GroupService>();
            services.AddTransient<LicensedProductService>();
            services.AddTransient<LoggingService>();

            ConfigureKeyVault(services);
            ConfigureCosmosDb(services);
        }

        private static void ConfigureKeyVault(IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var endpoint = configuration[ConfigurationNames.KeyVault.BaseUri];
                var endpointUrl = new Uri(endpoint);
                var managedIdentityClientId = configuration[ConfigurationNames.KeyVault.ManagedIdentityClient];

                var credentials = GetCredential(managedIdentityClientId);

                return new SecretClient(vaultUri: endpointUrl, credential: credentials);
            });

        }

        private static TokenCredential GetCredential(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return new DefaultAzureCredential();

            return new ManagedIdentityCredential(clientId);
        }

        private static void ConfigureCosmosDb(IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var secretClient = provider.GetService<SecretClient>();
                var configuration = provider.GetService<IConfiguration>();
                var endpoint = configuration[ConfigurationNames.Cosmos.BaseUri];

                var secret = secretClient.GetSecret(SecretNames.CosmosPrimaryKey);
                var key = secret.Value.Value;

                var client = GetCosmosClient(endpoint, key);

                return client;
            });

            services.AddTransient(provider =>
            {
                var client = provider.GetService<CosmosClient>();
                var queryFactory = provider.GetService<ICosmosLinqQueryFactory>();
                return new CosmosDbService(client, CosmosConfiguration.DatabaseId, queryFactory);
            });

            services.AddTransient<ICosmosLinqQueryFactory, CosmosLinqQueryFactory>();
        }

        private static CosmosClient GetCosmosClient(string endpoint, string key)
        {
            var options = new CosmosClientOptions
            {
                AllowBulkExecution = true,
                MaxRetryAttemptsOnRateLimitedRequests = 19,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromMinutes(1),
                SerializerOptions = new CosmosSerializationOptions
                {
                    IgnoreNullValues = true
                }
            };

            var client = new CosmosClient(endpoint, key, options);
            return client;
        }
    }
}
