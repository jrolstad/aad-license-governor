using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Orchestrators;
using Microsoft.Azure.Functions.Worker;

namespace AzureAdLicenseGovernor.Worker.Functions
{
    public class MonitorProductsFunctions
    {
        private readonly ProductMonitoringOrchestrator _orchestrator;

        public MonitorProductsFunctions(ProductMonitoringOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Function("licensed-product-monitor")]
        public Task Run([TimerTrigger("%ProductFunction_MonitorCron%")] TimerInfo myTimer)
        {
            return _orchestrator.Monitor();
        }
    }


}
