using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Orchestrators;
using Microsoft.Azure.Functions.Worker;

namespace AzureAdLicenseGovernor.Worker.Functions
{
    public class ApplyLicenseFunctions
    {
        private readonly LicensedGroupGovernanceOrchestrator _licensedGroupGovernanceOrchestrator;

        public ApplyLicenseFunctions(LicensedGroupGovernanceOrchestrator licensedGroupGovernanceOrchestrator)
        {
            _licensedGroupGovernanceOrchestrator = licensedGroupGovernanceOrchestrator;
        }

        [Function("license-governance-apply")]
        public Task Apply([TimerTrigger("%GroupLicenseFunction_ApplyCron%")] TimerInfo myTimer)
        {
            return _licensedGroupGovernanceOrchestrator.Apply();
        }
    }
}
