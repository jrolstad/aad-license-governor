using AzureAdLicenseGovernor.Tests.Shared;
using AzureAdLicenseGovernor.Worker.Configuration;
using AzureAdLicenseGovernor.Worker.Functions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Worker.Tests
{
    public class TestBuilder : TestBuilderBase
    {
        protected override void ConfigureApplication(IServiceCollection services)
        {
            var properties = new Dictionary<object, object>();
            var context = new HostBuilderContext(properties);
            DependencyInjectionConfiguration.Configure(context, services);
        }

        protected override void ConfigureFunctions(IServiceCollection services)
        {
            services.AddTransient<ApplyLicenseFunctions>();
        }
    }
}
