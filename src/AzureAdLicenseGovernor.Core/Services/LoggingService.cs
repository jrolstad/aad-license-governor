using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message,Dictionary<string,object> properties)
        {
            _logger.LogInformation(message);
        }
    }
}
