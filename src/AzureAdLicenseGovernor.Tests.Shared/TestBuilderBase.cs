using Azure.Security.KeyVault.Secrets;
using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Services;
using AzureAdLicenseGovernor.Tests.Shared.Extensions;
using AzureAdLicenseGovernor.Tests.Shared.Fakes;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Tests.Shared
{
    public abstract class TestBuilderBase
    {
        private static ServiceProvider _serviceProvider;

        public TestBuilderBase() : this(new ServiceCollection())
        {

        }
        private TestBuilderBase(IServiceCollection services)
        {
            Context = new TestContext();
            Configure(services);
        }

        private void Configure(IServiceCollection services)
        {
            ConfigureApplicationConfiguration(services);
            ConfigureApplication(services);
            ConfigureFunctionsWorker(services);
            ConfigureFunctions(services);
            ConfigureFakes(services);

            _serviceProvider = services.BuildServiceProvider();

            ClockService.Freeze();
        }

        private static void ConfigureApplicationConfiguration(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
        }

        private static void ConfigureFunctionsWorker(IServiceCollection services)
        {
            services.AddFunctionsWorkerCore();
            services.AddFunctionsWorkerDefaults();
        }

        protected abstract void ConfigureApplication(IServiceCollection services);

        protected abstract void ConfigureFunctions(IServiceCollection services);

        private void ConfigureFakes(IServiceCollection services)
        {
            services.AddTransient(provider => { return this.Context; });
            services.ReplaceTransient<CosmosClient, CosmosClientFake>();
            services.ReplaceTransient<SecretClient, SecretClientFake>();
            services.ReplaceTransient<ICosmosLinqQueryFactory, CosmosLinqQueryFactoryFake>();
            services.ReplaceTransient<IGraphClientFactory, GraphClientFactoryFake>();
            services.ReplaceTransient<ILoggingService, LoggingServiceFake>();
        }

        public T Get<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public void Dispose()
        {
            ClockService.Thaw();
        }

        public TestContext Context { get; private set; }
    }
}
