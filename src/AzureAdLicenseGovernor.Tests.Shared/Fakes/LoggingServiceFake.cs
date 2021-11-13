using AzureAdLicenseGovernor.Core.Services;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class LoggingServiceFake : ILoggingService
    {
        private readonly TestContext _context;

        public LoggingServiceFake(TestContext context)
        {
            _context = context;
        }
        public void LogInfo(string message, Dictionary<string, string> properties = null)
        {
            var entry = new LogEntry
            {
                Message = message,
                Data = properties
            };

            _context.Log.Logs.Add(entry);
        }

        public void LogMetric(string message, string name, double value, Dictionary<string, string> properties = null)
        {
            var entry = new LogEntry
            {
                Message = name,
                Metric = value,
                Data = properties
            };

            _context.Log.Logs.Add(entry);
        }
    }
}
