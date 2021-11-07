using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.Graph;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers
{
    public class GraphGroupHttpRequestHandler : IHttpRequestHandler
    {
        private TestContext _context;

        public GraphGroupHttpRequestHandler(TestContext context)
        {
            _context = context;
        }

        public bool AppliesTo(HttpRequestMessage request)
        {
            return request.Method == HttpMethod.Get && request.RequestUri.AbsoluteUri.StartsWith(GraphUri.Groups);
        }

        public Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
