using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class LoggingContext
    {
        public ConcurrentBag<LogEntry> Logs = new();
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public double? Metric { get; set; }
        public Dictionary<string,string> Data { get; set; }
    }
}
