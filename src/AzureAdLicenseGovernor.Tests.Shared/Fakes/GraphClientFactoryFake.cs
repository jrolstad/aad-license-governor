using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Tests.Shared.Fakes.Http;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class GraphClientFactoryFake : IGraphClientFactory
    {
        private readonly TestContext _context;

        public GraphClientFactoryFake(TestContext context)
        {
            _context = context;
        }

        public Task<IGraphServiceClient> CreateAsync(string directoryId)
        {
            return CreateAsync();
        }

        public Task<IGraphServiceClient> CreateAsync(Core.Models.Directory directory)
        {
            return CreateAsync();
        }

        public Task<IGraphServiceClient> CreateAsync()
        {
            var authenticationProvider = new AuthenticationProviderFake();
            var provider = new HttpProviderFake(_context);
            IGraphServiceClient graphClient = new GraphServiceClient(authenticationProvider, provider);

            return Task.FromResult(graphClient);
        }
    }
}
