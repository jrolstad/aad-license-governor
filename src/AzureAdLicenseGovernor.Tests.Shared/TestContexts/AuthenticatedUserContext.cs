using System.Security.Claims;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class AuthenticatedUserContext
    {
        public ClaimsIdentity AuthenticatedUser = new ClaimsIdentity();
    }
}
