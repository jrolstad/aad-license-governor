using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Orchestrators;
using Microsoft.Azure.Functions.Worker;

namespace AzureAdLicenseGovernor.Worker.Functions
{
    public class MonitorGroupsFunctions
    {
        private readonly LicensedGroupMonitoringOrchestrator _orchestrator;

        public MonitorGroupsFunctions(LicensedGroupMonitoringOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Function("license-governance-monitor")]
        public Task Run([TimerTrigger("%GroupLicenseFunction_MonitorCron%")] TimerInfo myTimer)
        {
            return _orchestrator.Monitor();
        }
    }


}
