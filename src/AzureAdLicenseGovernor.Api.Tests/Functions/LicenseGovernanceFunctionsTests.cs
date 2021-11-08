using AzureAdLicenseGovernor.Api.Functions;
using AzureAdLicenseGovernor.Api.Tests.Extensions;
using AzureAdLicenseGovernor.Core.Configuration.Authorization;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using AzureAdLicenseGovernor.Tests.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace AzureAdLicenseGovernor.Api.Tests.Functions
{
    public class LicenseGovernanceFunctionsTests
    {
        [Fact]
        public async Task Put_UnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();

            var function = builder.Get<LicenseGovernanceFunctions>();

            var result = await function.Put(builder.PutRequest("data"), builder.Context());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Put_AuthorizedUser_ReturnsOk()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.LicensingContributor);

            var function = builder.Get<LicenseGovernanceFunctions>();

            var result = await function.Put(builder.PutRequest("data"), builder.Context());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
