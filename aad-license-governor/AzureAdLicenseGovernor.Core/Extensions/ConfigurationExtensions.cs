using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsDevelopment(this IConfiguration configuration)
        {
            return configuration["ASPNETCORE_ENVIRONMENT"] == "Development";
        }
    }
}
