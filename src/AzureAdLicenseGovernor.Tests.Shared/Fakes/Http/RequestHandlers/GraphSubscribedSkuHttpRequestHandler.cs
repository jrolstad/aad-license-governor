using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.Graph;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers
{
    public class GraphSubscribedSkuHttpRequestHandler : IHttpRequestHandler
    {
        private TestContext _context;

        public GraphSubscribedSkuHttpRequestHandler(TestContext context)
        {
            _context = context;
        }

        public bool AppliesTo(HttpRequestMessage request)
        {
            return request.Method == HttpMethod.Get && request.RequestUri.AbsoluteUri.StartsWith(GraphUri.SubscribedSkus);
        }

        public Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request)
        {
            var responseData = new GraphResponse<ICollection<SubscribedSku>>
            {
                value = _context.GraphApi.SubscribedSkus.Values
            };

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseData))
            };
            return Task.FromResult(response);
        }
    }
}
