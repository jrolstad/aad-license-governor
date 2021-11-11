using AzureAdLicenseGovernor.Tests.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using Xunit;

namespace AzureAdLicenseGovernor.Worker.Tests.StepDefinitions
{
    [Binding]
    public class ThenSteps
    {
        private readonly TestBuilder _testBuilder;

        public ThenSteps(TestBuilder testBuilder)
        {
            _testBuilder = testBuilder;
        }

        [Then(@"the group '([^']*)' in tenant '([^']*)' has license assignments")]
        public void ThenTheGroupHasLicenseAssignments(string groupName, string tenantName, Table expectedAssignments)
        {
            var group = _testBuilder.GetGroup(groupName);

            Assert.Equal(expectedAssignments.RowCount, group.AssignedLicenses.Count());

            foreach(var row in expectedAssignments.Rows)
            {
                var product = GetProduct(row);
                var expectedDisabledPlanIds = GetExpectedDisabledPlans(row, product);

                var actualAssignment = group.AssignedLicenses.First(a => a.SkuId == product.SkuId);
                Assert.Equal(expectedDisabledPlanIds, actualAssignment.DisabledPlans);
            }

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

        [Then(@"the group '([^']*)' in tenant '([^']*)' has no license assignments")]
        public void ThenTheGroupInTenantHasNoLicenseAssignments(string groupName, string tenantName)
        {
            var group = _testBuilder.GetGroup(groupName);

            Assert.Empty(group.AssignedLicenses ?? new List<Microsoft.Graph.AssignedLicense>());
        }

        [Then(@"there is a '([^']*)' message logged with data")]
        public void ThenThereIsAMessageLoggedWithData(string message, Table data)
        {
            throw new PendingStepException();
        }

        [Then(@"there is not a '([^']*)' message logged")]
        public void ThenThereIsNotAMessageLogged(string message)
        {
            throw new PendingStepException();
        }



    }
}
