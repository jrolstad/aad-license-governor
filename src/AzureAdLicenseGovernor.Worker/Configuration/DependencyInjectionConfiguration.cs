using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureAdLicenseGovernor.Worker.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            Core.Configuration.DependencyInjection.DependencyInjectionConfiguration.Register(services);
        }
    }
}
