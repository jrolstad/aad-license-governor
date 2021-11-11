using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http
{
    public class HttpProviderFake : IHttpProvider
    {
        private readonly List<IHttpRequestHandler> _requestHandlers;

        public HttpProviderFake(TestContext context)
        {
            _requestHandlers = new List<IHttpRequestHandler>
            {
                new GraphSubscribedSkuHttpRequestHandler(context),
                new GraphGroupHttpRequestHandler(context),
                new GraphGroupAssignedLicenseHttpRequestHandler(context),
                new GraphGroupSearchHttpRequestHandler(context),
            };
        }

        public ISerializer Serializer => new Serializer();

        public TimeSpan OverallTimeout { get; set; } = TimeSpan.FromMinutes(10);

        public void Dispose()
        {

        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var handler = _requestHandlers.FirstOrDefault(h => h.AppliesTo(request));
            if (handler != null)
            {
                var result = await handler.ProcessAsync(request);
                return result;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            var handler = _requestHandlers.FirstOrDefault(h => h.AppliesTo(request));
            if (handler != null)
            {
                var result = await handler.ProcessAsync(request);
                return result;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
