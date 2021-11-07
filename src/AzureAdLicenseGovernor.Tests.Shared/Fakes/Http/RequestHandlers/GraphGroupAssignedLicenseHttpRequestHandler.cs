using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.Graph;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes.Http.RequestHandlers
{
    public class GraphGroupAssignedLicenseHttpRequestHandler : IHttpRequestHandler
    {
        private TestContext _context;

        public GraphGroupAssignedLicenseHttpRequestHandler(TestContext context)
        {
            _context = context;
        }

        public bool AppliesTo(HttpRequestMessage request)
        {
            return request.Method == HttpMethod.Post && 
                request.RequestUri.AbsoluteUri.StartsWith(GraphUri.Groups) &&
                request.RequestUri.AbsoluteUri.EndsWith("assignLicense",StringComparison.OrdinalIgnoreCase);
        }

        public async Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request)
        {
            var requestParts = request.RequestUri.AbsoluteUri.Replace(GraphUri.Groups, "").Split("/");
            var id = requestParts.FirstOrDefault();

            if (_context.GraphApi.Groups.TryGetValue(id, out Microsoft.Graph.Group group))
            {
                var requestData = await ParseRequest(request);
                RemoveLicenses(group, requestData);
                AddLicenses(group, requestData);
 
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(group))
                };
                return response;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        private async Task<AssignLicenseRequest> ParseRequest(HttpRequestMessage request)
        {
            var rawContent = await request.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<AssignLicenseRequest>(rawContent);

            return data;
        }
        private static void AddLicenses(Microsoft.Graph.Group group, AssignLicenseRequest requestData)
        {
            var assigned = group.AssignedLicenses.ToList();
            assigned.AddRange(requestData.AddLicenses);
            group.AssignedLicenses = assigned;
        }

        private static void RemoveLicenses(Microsoft.Graph.Group group, AssignLicenseRequest requestData)
        {
            var assigned = group.AssignedLicenses.ToList();

            var toRemove = assigned
                .Where(a => requestData.RemoveLicenses.Contains(a.SkuId.GetValueOrDefault()))
                .ToList();

            foreach(var item in toRemove)
            {
                assigned.Remove(item);
            }
            
            group.AssignedLicenses = assigned;
        }

        private class AssignLicenseRequest
        {
            public List<Microsoft.Graph.AssignedLicense> AddLicenses { get; set; }
            public List<Guid> RemoveLicenses { get; set; }
        }

        
    }
}
