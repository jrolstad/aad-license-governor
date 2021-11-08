using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.IO;

namespace AzureAdLicenseGovernor.Api.Tests.Extensions
{
    public static class HttpResponseDataExtensions
    {
        public static T? Value<T>(this HttpResponseData response)
        {
            if (response == null) return default;

            response.Body.Position = 0;
            var reader = new StreamReader(response.Body);
            var data = reader.ReadToEnd();

            var result = JsonConvert.DeserializeObject<T>(data);

            return result;
        }
    }
}
