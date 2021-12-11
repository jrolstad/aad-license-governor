using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Extensions
{
    public static class GraphExtensions
    {
        public static async Task<IEnumerable<TResponse>> ExecuteBatch<TResponse>(this Microsoft.Graph.IGraphServiceClient client,
          IEnumerable<Microsoft.Graph.IBaseRequest> requests)
        {
            if(requests == null || !requests.Any()) return Enumerable.Empty<TResponse>();

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
