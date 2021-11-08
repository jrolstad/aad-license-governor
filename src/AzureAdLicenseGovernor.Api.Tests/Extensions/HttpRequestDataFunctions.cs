using AzureAdLicenseGovernor.Tests.Shared.Extensions;
using AzureAdLicenseGovernor.Tests.Shared.Fakes;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net.Http;

namespace AzureAdLicenseGovernor.Api.Tests.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static HttpRequestData GetRequest(this TestBuilder testBuilder)
        {
            return new HttpRequestDataFake(testBuilder.Context(), HttpMethod.Get, testBuilder.Context);
        }

        public static HttpRequestData PostRequest<T>(this TestBuilder testBuilder, T body)
        {
            var request = new HttpRequestDataFake(testBuilder.Context(), HttpMethod.Post, testBuilder.Context);
            request.SetBody(body);

            return request;
        }

        public static HttpRequestData PutRequest<T>(this TestBuilder testBuilder, T body)
        {
            var request = new HttpRequestDataFake(testBuilder.Context(), HttpMethod.Put, testBuilder.Context);
            request.SetBody(body);

            return request;
        }

        public static HttpRequestData DeleteRequest(this TestBuilder testBuilder)
        {
            return new HttpRequestDataFake(testBuilder.Context(), HttpMethod.Delete, testBuilder.Context);
        }
    }
}
