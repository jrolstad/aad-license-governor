using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupGovernanceOrchestrator
    {
        private readonly LicensedGroupOrchestrator _licensedGroupOrchestrator;

        public LicensedGroupGovernanceOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
        }

        public Task Apply()
        {
            var groups = _licensedGroupOrchestrator.Get();

            return Task.CompletedTask;
        }
    }
}
