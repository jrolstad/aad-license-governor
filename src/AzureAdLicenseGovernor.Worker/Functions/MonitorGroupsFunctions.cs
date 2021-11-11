using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Orchestrators;
using Microsoft.Azure.Functions.Worker;

namespace AzureAdLicenseGovernor.Worker.Functions
{
    public class MonitorGroupsFunctions
    {
        private readonly LicensedGroupMonitoringOrchestrator _licensedGroupMonitoringOrchestrator;

        public MonitorGroupsFunctions(LicensedGroupMonitoringOrchestrator licensedGroupMonitoringOrchestrator)
        {
            _licensedGroupMonitoringOrchestrator = licensedGroupMonitoringOrchestrator;
        }

        [Function("license-governance-monitor")]
        public Task Run([TimerTrigger("%GroupLicenseFunction_MonitorCron%")] TimerInfo myTimer)
        {
            return _licensedGroupMonitoringOrchestrator.Monitor();
        }
    }

    
}
