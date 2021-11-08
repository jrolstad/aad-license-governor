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
    public class GroupFunctionsTests
    {
        [Fact]
        public async Task Get_UnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();

            var function = builder.Get<GroupFunctions>();

            var result = await function.Get(builder.GetRequest(), builder.Context());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);

        }

        [Fact]
        public async Task Get_AuthorizedUser_ReturnsGroups()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.GroupContributor);
            var directory = builder.WithDirectory(name: "test-domain1");

            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var group2 = builder.WithGroup(directory.TenantId, "the-group2");

            var product1 = builder.WithLicensedProduct("product-1");
            var servicePlan1 = builder.WithServicePlan(product1.Id, "plan-1");
            var servicePlan2 = builder.WithServicePlan(product1.Id, "plan-2");

            var product2 = builder.WithLicensedProduct("product-2");
            var servicePlan3 = builder.WithServicePlan(product2.Id, "plan-3");

            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);
            builder.WithProductAssignment(licensedGroup1.TenantId, licensedGroup1.ObjectId, product1.SkuPartNumber, servicePlan1.ServicePlanName, servicePlan2.ServicePlanName);
            
            var licensedGroup2 = builder.WithLicensedGroup(directory.TenantId, group2.Id);
            builder.WithProductAssignment(licensedGroup2.TenantId, licensedGroup2.ObjectId, product2.SkuPartNumber, servicePlan3.ServicePlanName);

            var function = builder.Get<GroupFunctions>();

            var result = await function.Get(builder.GetRequest(), builder.Context());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var value = result.Value<List<LicensedGroup>>();

            Assert.Equal(2, value?.Count);

            AssertData(value.First(v => v.ObjectId == licensedGroup1.ObjectId), licensedGroup1);
            AssertData(value.First(v => v.ObjectId == licensedGroup2.ObjectId), licensedGroup2);

        }

        [Fact]
        public async Task Get_ValidIdAndUnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();

            var directory = builder.WithDirectory(name: "test-domain1");
            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);

            var function = builder.Get<GroupFunctions>();

            var result = await function.GetById(builder.GetRequest(), builder.Context(), licensedGroup1.TenantId,licensedGroup1.ObjectId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);

        }

        [Fact]
        public async Task Get_InvalidIdAndAuthorizedUser_ReturnsNotFound()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.GroupContributor);

            var directory = builder.WithDirectory(name: "test-domain1");
            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);

            var function = builder.Get<GroupFunctions>();

            var result = await function.GetById(builder.GetRequest(), builder.Context(), licensedGroup1.TenantId, "not-group-1");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Get_ValidIdAndAuthorizedUser_ReturnsGroup()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.GroupContributor);
            var directory = builder.WithDirectory(name: "test-domain1");

            var group1 = builder.WithGroup(directory.TenantId, "the-group1");

            var product1 = builder.WithLicensedProduct("product-1");
            var servicePlan1 = builder.WithServicePlan(product1.Id, "plan-1");
            var servicePlan2 = builder.WithServicePlan(product1.Id, "plan-2");

            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);
            builder.WithProductAssignment(licensedGroup1.TenantId, licensedGroup1.ObjectId, product1.SkuPartNumber, servicePlan1.ServicePlanName, servicePlan2.ServicePlanName);

            var function = builder.Get<GroupFunctions>();

            var result = await function.GetById(builder.GetRequest(), builder.Context(),licensedGroup1.TenantId,licensedGroup1.ObjectId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var value = result.Value<LicensedGroup>();

            Assert.NotNull(value);
            AssertData(value, licensedGroup1);

        }

        [Fact]
        public async Task Delete_ValidIdAndUnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();

            var directory = builder.WithDirectory(name: "test-domain1");
            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);

            var function = builder.Get<GroupFunctions>();

            var result = await function.Delete(builder.DeleteRequest(), builder.Context(), licensedGroup1.TenantId, licensedGroup1.ObjectId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);

        }

        [Fact]
        public async Task Post_UnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();
            var directory = builder.WithDirectory(name: "test-domain1");

            var function = builder.Get<GroupFunctions>();

            var data = GetGroup(directory.TenantId);

            var result = await function.Post(builder.PostRequest(data), builder.Context());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Post_AuthorizedUser_SavesData()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.GroupContributor);
            var directory = builder.WithDirectory(name: "test-domain1");

            var function = builder.Get<GroupFunctions>();

            var data = GetGroup(directory.TenantId);

            var result = await function.Post(builder.PostRequest(data), builder.Context());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            AssertData(data, builder.Context.Data.Groups.Values.First(),false);
        }

        [Fact]
        public async Task Put_UnauthorizedUser_ReturnsForbidden()
        {
            var builder = new TestBuilder();
            var directory = builder.WithDirectory(name: "test-domain1");
            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);

            var function = builder.Get<GroupFunctions>();

            var data = GetGroup(directory.TenantId);
            data.ObjectId = licensedGroup1.ObjectId;
            data.TenantId = licensedGroup1.TenantId;

            var result = await function.Put(builder.PutRequest(data), builder.Context(),licensedGroup1.TenantId,licensedGroup1.ObjectId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Put_AuthorizedUser_SavesData()
        {
            var builder = new TestBuilder();
            builder.WithAuthenticatedUser("the-user", Roles.GroupContributor);

            var directory = builder.WithDirectory(name: "test-domain1");
            var group1 = builder.WithGroup(directory.TenantId, "the-group1");
            var licensedGroup1 = builder.WithLicensedGroup(directory.TenantId, group1.Id);

            var function = builder.Get<GroupFunctions>();

            var data = GetGroup(directory.TenantId);
            data.ObjectId = licensedGroup1.ObjectId;
            data.TenantId = licensedGroup1.TenantId;

            var result = await function.Put(builder.PutRequest(data), builder.Context(), licensedGroup1.TenantId, licensedGroup1.ObjectId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            AssertData(data, builder.Context.Data.Groups.Values.First());
        }

        private LicensedGroup GetGroup(string tenantId, 
            ProductAssignmentMode mode = ProductAssignmentMode.Enforce)
        {
            return new LicensedGroup
            {
                ObjectId = Guid.NewGuid().ToString(),
                TenantId = tenantId,
                Mode = mode,
                LicensedProducts = new List<ProductAssignment>
                {
                    new ProductAssignment
                    {
                        Id = Guid.NewGuid().ToString(),
                        EnabledServicePlans = new List<string>
                        {
                            Guid.NewGuid().ToString()
                        }
                    }
                }
            };
        }

        private void AssertData(LicensedGroup actual, LicensedGroupData expected, bool validateId = true)
        {
            if (validateId)
            {
                Assert.Equal(expected.Id, $"{actual.TenantId}|{actual.ObjectId}");
            }

            Assert.Equal(expected.TenantId, actual.TenantId);
            Assert.Equal(expected.ObjectId, actual.ObjectId);

            AssertLicensedProducts(actual, expected);

        }

        private static void AssertLicensedProducts(LicensedGroup actual, LicensedGroupData expected)
        {
            Assert.Equal(expected.LicensedProducts.Count, actual.LicensedProducts.Count);

            var actualProductsById = actual.LicensedProducts.ToDictionary(p => p.Id);
            foreach (var expectedProduct in expected.LicensedProducts)
            {
                var actualProduct = actualProductsById[expectedProduct.Id];

                Assert.Equal(expectedProduct.EnabledServicePlans, actualProduct.EnabledServicePlans);
            }
        }
    }
}
