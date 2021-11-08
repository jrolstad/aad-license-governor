using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            Core.Configuration.DependencyInjection.DependencyInjectionConfiguration.Register(services);
        }
    }
}
