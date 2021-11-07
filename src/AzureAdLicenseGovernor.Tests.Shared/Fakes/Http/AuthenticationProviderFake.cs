using System.Net.Http;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http
{
    public class AuthenticationProviderFake : Microsoft.Graph.IAuthenticationProvider
    {
        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }
    }
}
