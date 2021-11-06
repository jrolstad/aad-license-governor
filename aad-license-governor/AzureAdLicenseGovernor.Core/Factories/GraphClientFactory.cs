using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Orchestrators;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Factories
{
    public interface IGraphClientFactory
    {
        Task<IGraphServiceClient> CreateAsync(Models.Directory directory);
    }

    public class GraphClientFactory : IGraphClientFactory
    {
        private readonly SecretClient _secretService;
        private readonly DirectoryOrchestrator _directoryManager;
        private readonly IConfiguration _configurationService;

        public GraphClientFactory(SecretClient secretService,
            DirectoryOrchestrator directoryManager,
            IConfiguration configurationService)
        {
            _secretService = secretService;
            _directoryManager = directoryManager;
            _configurationService = configurationService;
        }

        public Task<IGraphServiceClient> CreateAsync(Models.Directory directory)
        {
            var graphUri = new Uri(directory.GraphUrl);
            var baseUri = new Uri(graphUri, "beta");
            var scopeUri = new Uri(graphUri, ".default");

            var graphServiceClient = new GraphServiceClient(baseUri.ToString(),
                new DelegateAuthenticationProvider(async requestMessage =>
                {
                    var credential = GetCredential(directory);
                    var scope = scopeUri.ToString();
                    var token = await GetAccessToken(credential, scope);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                }));

            return Task.FromResult<IGraphServiceClient>(graphServiceClient);
        }

        private TokenCredential GetCredential(Models.Directory directory)
        {
            switch (directory.ClientType)
            {
                case DirectoryClientType.ManagedIdentity:
                    {
                        return new ManagedIdentityCredential(clientId: directory.ClientId);
                    }
                case DirectoryClientType.Application:
                    {
                        var clientSecret = _secretService.GetSecret($"directory-{directory.Id}-clientsecret").Value.Value;
                        return new ClientSecretCredential(tenantId: directory.TenantId, clientId: directory.ClientId, clientSecret: clientSecret);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(directory), directory.ClientType, "Unsupported AuthenticationType");
                    }
            }

        }

        private async Task<string> GetAccessToken(TokenCredential tokenCredential, params string[] scopes)
        {
            var accessToken = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes), new CancellationToken());

            return accessToken.Token;
        }
    }
}
