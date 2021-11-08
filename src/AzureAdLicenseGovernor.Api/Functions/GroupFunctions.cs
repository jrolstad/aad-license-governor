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
    public class GroupFunctions
    {
        private readonly LicensedGroupOrchestrator _licensedGroupOrchestrator;
        private readonly AuthorizationService _authorizationService;

        public GroupFunctions(LicensedGroupOrchestrator licensedGroupOrchestrator,
            AuthorizationService authorizationService)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _authorizationService = authorizationService;
        }

        [Function("group-get")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "group")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ViewGroups, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _licensedGroupOrchestrator.Get();

            return await req.OkResponseAsync(data);
        }

        [Function("group-getbyid")]
        public async Task<HttpResponseData> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "group/{tenantId}/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string tenantId,
            string id)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ViewGroups, req.GetRequestingUser())) return req.UnauthorizedResponse();

            var data = await _licensedGroupOrchestrator.Get(tenantId,id);

            if (string.IsNullOrEmpty(data?.ObjectId)) return req.NotFoundResponse();

            return await req.OkResponseAsync(data);
        }

        [Function("group-post")]
        public async Task<HttpResponseData> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "group/")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageGroups, req.GetRequestingUser())) return req.UnauthorizedResponse();

            await _licensedGroupOrchestrator.Add(req.GetBody<LicensedGroup>());

            return await req.OkResponseAsync();
        }

        [Function("group-put")]
        public async Task<HttpResponseData> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "group/{tenantId}/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string tenantId,
            string id)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageGroups, req.GetRequestingUser())) return req.UnauthorizedResponse();

            await _licensedGroupOrchestrator.Update(tenantId,id, req.GetBody<LicensedGroup>());

            return await req.OkResponseAsync();
        }

        [Function("group-delete")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "group/{tenantId}/{id}")]
            HttpRequestData req,
            FunctionContext executionContext,
            string tenantId,
            string id)
        {
            if (!_authorizationService.IsAuthorized(AuthorizedActions.ManageGroups, req.GetRequestingUser())) return req.UnauthorizedResponse();

            await _licensedGroupOrchestrator.Delete(tenantId,id);

            return await req.OkResponseAsync();
        }
    }
}
