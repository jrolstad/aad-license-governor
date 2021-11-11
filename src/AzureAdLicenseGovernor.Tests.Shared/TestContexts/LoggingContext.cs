using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class LoggingContext
    {
        public List<LogEntry> Logs = new();
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public double? Metric { get; set; }
        public Dictionary<string,string> Data { get; set; }
    }
}
