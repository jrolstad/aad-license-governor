using AzureAdLicenseGovernor.Api.Configuration;
using AzureAdLicenseGovernor.Api.Functions;
using AzureAdLicenseGovernor.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Api.Tests
{
    public class TestBuilder:TestBuilderBase
    {
        protected override void ConfigureApplication(IServiceCollection services)
        {
            var properties = new Dictionary<object, object>();
            var context = new HostBuilderContext(properties);
            DependencyInjectionConfiguration.Configure(context, services);
        }

        protected override void ConfigureFunctions(IServiceCollection services)
        {
            services.AddTransient<DirectoryFunctions>();
            services.AddTransient<GroupFunctions>();
            services.AddTransient<LicenseGovernanceFunctions>();
        }
    }
}
