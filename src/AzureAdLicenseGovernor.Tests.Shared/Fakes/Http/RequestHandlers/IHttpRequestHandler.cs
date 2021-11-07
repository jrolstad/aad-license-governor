using System.Net.Http;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers
{
    public interface IHttpRequestHandler
    {
        bool AppliesTo(HttpRequestMessage request);

        Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request);
    }
}
