using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Tests.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace AzureAdLicenseGovernor.Worker.Tests.StepDefinitions
{
    [Binding]
    public class GivenSteps
    {
        private readonly TestBuilder _testBuilder;

        public GivenSteps(TestBuilder testBuilder)
        {
            _testBuilder = testBuilder;
        }

        [Given(@"the Azure Active Directory Tenant '([^']*)'")]
        public void GivenTheAzureActiveDirectoryTenant(string tenantName)
        {
            _testBuilder.WithDirectory(tenantName);
        }

        [Given(@"licensed products in '([^']*)'")]
        public void GivenLicensedProductsIn(string tenantName, Table products)
        {
            foreach(var row in products.Rows)
            {
                var product = MapProduct(row);
                _testBuilder.WithLicensedProduct(product.SkuPartNumber, product.SkuId);
            }
        }

        private Microsoft.Graph.SubscribedSku MapProduct(TableRow row)
        {
            return new Microsoft.Graph.SubscribedSku
            {
                SkuId = Guid.Parse(row["SkuId"]),
                SkuPartNumber = row["SkuPartNumber"]
            };
        }

        [Given(@"service plans in '([^']*)' for product '([^']*)'")]
        public void GivenServicePlansInForProduct(string tenantName, string productName, Table servicePlans)
        {
            var product = _testBuilder.GetProduct(productName);

            foreach(var row in servicePlans.Rows)
            {
                var servicePlan = MapServicePlan(row);
                _testBuilder.WithServicePlan(product.Id, servicePlan.ServicePlanName, servicePlan.ServicePlanId);
            }
        }

        private Microsoft.Graph.ServicePlanInfo MapServicePlan(TableRow row)
        {
            return new Microsoft.Graph.ServicePlanInfo
            {
                ServicePlanId = Guid.Parse(row["ServicePlanId"]),
                ServicePlanName = row["ServicePlanName"]
            };
        }

        [Given(@"groups in tenant '([^']*)'")]
        public void GivenGroupsIn(string tenantName, Table groups)
        {
            var directory = _testBuilder.GetDirectory(tenantName);

            foreach(var group in groups.Rows)
            {
                var groupName = group["DisplayName"];
                _testBuilder.WithGroup(directory.TenantId, groupName);
            }
        }

        [Given(@"license configuration in '([^']*)' mode for group '([^']*)' in tenant '([^']*)'")]
        public void GivenLicenseConfigurationInModeForGroup(string modeName, string groupName, string tenantName, Table assignments)
        {
            var directory = _testBuilder.GetDirectory(tenantName);
            var mode = Enum.Parse<ProductAssignmentMode>(modeName);
            var group = _testBuilder.GetGroup(groupName);

            var licensedGroup = _testBuilder.WithLicensedGroup(directory.TenantId, group.Id, mode);

            foreach(var assignment in assignments.Rows)
            {
                var productName = assignment["Product"];
                var enabledServicePlans = assignment["Enabled Features"]
                    .Split(",")
                    .Select(s=>s.Trim())
                    .ToArray();
                _testBuilder.WithProductAssignment(licensedGroup.TenantId, licensedGroup.ObjectId, productName, enabledServicePlans);
            }
        }

        [Given(@"the group '([^']*)' in tenant '([^']*)' has no license assignments")]
        public void GivenTheGroupInTenantHasNoLicenseAssignments(string groupName, string tenantName)
        {
            var group = _testBuilder.GetGroup(groupName);
            if(group!=null)
            {
                group.AssignedLicenses = new List<Microsoft.Graph.AssignedLicense>();
            }
        }

        [Given(@"the group '([^']*)' in tenant '([^']*)' has license assignments")]
        public void GivenTheGroupInTenantHasLicenseAssignments(string groupName, string tenantName, Table assignments)
        {
            var group = _testBuilder.GetGroup(groupName);

            var assignedLicenses = new List<Microsoft.Graph.AssignedLicense>();
            foreach(var productAssignment in assignments.Rows)
            {
                var product = GetProduct(productAssignment);

                var assignment = new Microsoft.Graph.AssignedLicense
                {
                    SkuId = product.SkuId,
                    DisabledPlans = GetExpectedDisabledPlans(productAssignment, product)
                };

                assignedLicenses.Add(assignment);
            }
            group.AssignedLicenses = assignedLicenses;

        }

        private Microsoft.Graph.SubscribedSku GetProduct(TableRow row)
        {
            var name = row["Product"];
            var product = _testBuilder.GetProduct(name);

            return product;
        }

        private static List<Guid> GetExpectedDisabledPlans(TableRow row, Microsoft.Graph.SubscribedSku product)
        {
            var expectedDisabledPlanNames = row["Disabled Features"]
                .Split(",")
                .Select(s => s.Trim());

            var expectedDisabledPlanIds = product.ServicePlans
                .Where(s => expectedDisabledPlanNames.Contains(s.ServicePlanName, StringComparer.OrdinalIgnoreCase))
                .Select(s => s.ServicePlanId.GetValueOrDefault())
                .ToList();
            return expectedDisabledPlanIds;
        }

        [Given(@"the governed group '([^']*)' is tracking license processing state")]
        public void GivenTheGovernedGroupIsTrackingLicenseProcessingState(string groupName)
        {
            var group = _testBuilder.GetGovernedGroup(groupName);
            group.TrackLicenseProcessingState = true;
        }

        [Given(@"the governed group '([^']*)' is not tracking license processing state")]
        public void GivenTheGovernedGroupIsNotTrackingLicenseProcessingState(string groupName)
        {
            var group = _testBuilder.GetGovernedGroup(groupName);
            group.TrackLicenseProcessingState = false;
        }



    }
}
