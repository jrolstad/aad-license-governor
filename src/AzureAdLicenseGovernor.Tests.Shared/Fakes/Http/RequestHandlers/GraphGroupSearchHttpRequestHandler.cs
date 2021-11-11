using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.Graph;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers
{
    public class GraphGroupSearchHttpRequestHandler : IHttpRequestHandler
    {
        private TestContext _context;

        public GraphGroupSearchHttpRequestHandler(TestContext context)
        {
            _context = context;
        }

        public bool AppliesTo(HttpRequestMessage request)
        {
            return request.Method == HttpMethod.Get &&
               request.RequestUri.AbsoluteUri.StartsWith(GraphUri.Groups.TrimEnd('/')) &&
               request.RequestUri.Query.Contains("filter=");
        }

        public Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request)
        {
            var data = new List<Group>();

            var encodedQuery = Uri.EscapeDataString("hasMembersWithLicenseErrors eq true");
            if (request.RequestUri.Query.Contains(encodedQuery, StringComparison.OrdinalIgnoreCase))
            {
                data = _context.GraphApi
                    .Groups
                    .Values
                    .Where(g => g.HasMembersWithLicenseErrors == true)
                    .ToList();
            }

            var responseData = new GraphResponse<ICollection<Group>>
            {
                value = data
            };

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseData))
            };
            return Task.FromResult(response);
        }
    }
}
