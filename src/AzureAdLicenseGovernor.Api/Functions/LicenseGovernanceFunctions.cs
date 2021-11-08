using System.Threading.Tasks;
using AzureAdLicenseGovernor.Api.Extensions;
using AzureAdLicenseGovernor.Core.Configuration.Authorization;
using AzureAdLicenseGovernor.Core.Orchestrators;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureAdLicenseGovernor.Api.Functions
{
    public class LicenseGovernanceFunctions
    {
        private readonly LicensedGroupGovernanceOrchestrator _licensedGroupGovernanceOrchestrator;
        private readonly AuthorizationService _authorizationService;

        public LicenseGovernanceFunctions(LicensedGroupGovernanceOrchestrator licensedGroupGovernanceOrchestrator,
            AuthorizationService authorizationService)
        {
            _licensedGroupGovernanceOrchestrator = licensedGroupGovernanceOrchestrator;
            _authorizationService = authorizationService;
        }

        [Function("licensegovernance-put")]
        public async Task<HttpResponseData> Put(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "license/governance/apply")]
            HttpRequestData req,
           FunctionContext executionContext)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ApplyLicensing, req.GetRequestingUser())) return req.UnauthorizedResponse();

            await _licensedGroupGovernanceOrchestrator.Apply();

            return await req.OkResponseAsync();
        }
    }
}
