using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Services
{
    public interface ILoggingService
    {
        void LogInfo(string message, Dictionary<string, string> properties = null);
        void LogMetric(string name, double value, Dictionary<string, string> properties = null);
    }

    public class LoggingService: ILoggingService
    {
        private TelemetryClient _logger;

        public LoggingService(TelemetryConfiguration telemetryConfiguration)
        {
            _logger = new TelemetryClient(telemetryConfiguration);
        }

        public void LogInfo(string message,Dictionary<string,string> properties=null)
        {
            _logger.TrackTrace(message, properties);
        }

        public void LogMetric(string name, double value, Dictionary<string, string> properties=null)
        {
            _logger.TrackMetric(name, value, properties);
        }
    }
}
