using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Api.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static IEnumerable<ClaimsIdentity> GetRequestingUser(this HttpRequestData request)
        {
            return request.Identities;
        }

        public static HttpResponseData UnauthorizedResponse(this HttpRequestData request)
        {
            return request.CreateResponse(HttpStatusCode.Forbidden);
        }
        public static Task<HttpResponseData> UnauthorizedResponseAsync(this HttpRequestData request)
        {
            return Task.FromResult(request.CreateResponse(HttpStatusCode.Forbidden));
        }

        public static Task<HttpResponseData> OkResponseAsync(this HttpRequestData request)
        {
            return Task.FromResult(request.CreateResponse(HttpStatusCode.OK));
        }

        public static async Task<HttpResponseData> OkResponseAsync<T>(this HttpRequestData request, T data)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(data);

            return response;
        }

        public static HttpResponseData NotFoundResponse(this HttpRequestData request)
        {
            return request.CreateResponse(HttpStatusCode.NotFound);
        }

        public static Task<HttpResponseData> NotFoundResponseAsync(this HttpRequestData request)
        {
            return Task.FromResult(request.CreateResponse(HttpStatusCode.NotFound));
        }

        public static T GetBody<T>(this HttpRequestData request)
        {
            if (request.Body == null) return default(T);

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body);
            var bodyAsString = reader.ReadToEnd();

            var result = JsonConvert.DeserializeObject<T>(bodyAsString);

            return result;
        }
    }
}
