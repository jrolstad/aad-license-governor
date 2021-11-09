using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.Graph;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Newtonsoft.Json;
using System;
using System.Linq;
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
            return request.Method == HttpMethod.Get && 
                request.RequestUri.AbsoluteUri.StartsWith(GraphUri.Groups);
        }

        public Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request)
        {
            var requestParts = request.RequestUri.AbsoluteUri.Replace(GraphUri.Groups, "").Split("/");
            var id = requestParts
                .FirstOrDefault()
                .Split("?")
                .FirstOrDefault();

            if(_context.GraphApi.Groups.TryGetValue(id,out Microsoft.Graph.Group group))
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(group))
                };
                return Task.FromResult(response);
            }

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }
    }
}
