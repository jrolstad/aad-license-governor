using System.Threading.Tasks;
using AzureAdLicenseGovernor.Api.Extensions;
using AzureAdLicenseGovernor.Core.Configuration.Authorization;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Orchestrators;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureAdLicenseGovernor.Api.Functions
{
    public class DirectoryFunctions
    {
        private readonly DirectoryOrchestrator _directoryOrchestrator;
        private readonly AuthorizationService _authorizationService;

        public DirectoryFunctions(DirectoryOrchestrator directoryOrchestrator,
            AuthorizationService authorizationService)
        {
            _directoryOrchestrator = directoryOrchestrator;
            _authorizationService = authorizationService;
        }

        [Function("directory-get")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "directory")]
            HttpRequestData req,
            FunctionContext executionContext)
        {

            if (!_authorizationService.IsAuthorized(AuthorizedActions.ViewDirectories, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _directoryOrchestrator.Get();

            return await req.OkResponseAsync(data);
        }

        [Function("directory-getbyid")]
        public async Task<HttpResponseData> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "directory/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string id)
        {

            if (!_authorizationService.IsAuthorized(AuthorizedActions.ViewDirectories, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _directoryOrchestrator.GetById(id);

            if (string.IsNullOrEmpty(data?.Id)) return req.NotFoundResponse();

            return await req.OkResponseAsync(data);
        }

        [Function("directory-post")]
        public async Task<HttpResponseData> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "directory/")]
            HttpRequestData req,
            FunctionContext executionContext)
        {

            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageDirectories, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _directoryOrchestrator.Add(req.GetBody<Directory>());

            return await req.OkResponseAsync(data);
        }

        [Function("directory-put")]
        public async Task<HttpResponseData> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "directory/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string id)
        {

            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageDirectories, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _directoryOrchestrator.Update(id, req.GetBody<Directory>());

            if (string.IsNullOrEmpty(data?.Id)) return req.NotFoundResponse();

            return await req.OkResponseAsync(data);
        }

        [Function("directory-delete")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "directory/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string id)
        {

            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageDirectories, req.GetRequestingUser())) return req.UnauthorizedResponse();

            await _directoryOrchestrator.Delete(id);

            return await req.OkResponseAsync();
        }
    }
}
