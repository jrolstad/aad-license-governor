using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using AzureAdLicenseGovernor.Api.Configuration;

namespace AzureAdLicenseGovernor.Api
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(DependencyInjectionConfiguration.Configure)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddEnvironmentVariables();
                })
                .Build();

            host.Run();
        }
    }
}