using AzureAdLicenseGovernor.Core.Extensions;
using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class UserService
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly UserMapper _userMapper;

        public UserService(IGraphClientFactory graphClientFactory, UserMapper userMapper)
        {
            _graphClientFactory = graphClientFactory;
            _userMapper = userMapper;
        }

        public async Task<ICollection<Models.User>> GetUserLicenseAssignmentStates(Models.Directory directory, IEnumerable<string> userIds)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var requests = userIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(id => client.Users[id]
                                    .Request()
                                    .Select("id,userPrincipalName,licenseAssignmentStates")
                                    );

            var userData = await ExecuteBatch<Microsoft.Graph.User>(client, requests);

            var result = userData
                .AsParallel()
                .Select(u => _userMapper.Map(directory, u))
                .ToList();

            return result;
        }

        public static async Task<IEnumerable<TResponse>> ExecuteBatch<TResponse>(Microsoft.Graph.IGraphServiceClient client,
           IEnumerable<Microsoft.Graph.IBaseRequest> requests)
        {
            const int maximumBatchSize = 20; // Microsoft Graph API Batch Endpoint has a maximum of 20 requests in a single batch request
            var groupedRequests = requests.Split(maximumBatchSize);

            var resultTasks = groupedRequests
                .Select(group => ExecuteBatchRequest<TResponse>(client, CreateBatchRequest(group)));
            var resultData = await Task.WhenAll(resultTasks);
            var results = resultData.SelectMany(r => r);

            return results;
        }

        private static Microsoft.Graph.BatchRequestContent CreateBatchRequest(IEnumerable<Microsoft.Graph.IBaseRequest> requests)
        {
            var batchedRequest = new Microsoft.Graph.BatchRequestContent();

            foreach (var request in requests)
            {
                batchedRequest.AddBatchRequestStep(request);
            }

            return batchedRequest;
        }

        private static async Task<IEnumerable<TResponse>> ExecuteBatchRequest<TResponse>(Microsoft.Graph.IGraphServiceClient client, Microsoft.Graph.BatchRequestContent batchedRequest)
        {
            var batchResponse = await client.Batch.Request().PostAsync(batchedRequest);
            var responses = await batchResponse.GetResponsesAsync();

            var resultTasks = responses
                .Select(r => batchResponse.GetResponseByIdAsync<TResponse>(r.Key));
            var result = await Task.WhenAll(resultTasks);

            return result;
        }
    }
}
