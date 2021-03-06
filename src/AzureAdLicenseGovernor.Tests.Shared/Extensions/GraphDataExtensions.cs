using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class GraphDataExtensions
    {
        public static SubscribedSku WithLicensedProduct(this TestBuilderBase root,
           string skuPartNumber,
           Guid? skuId=null,
           int? unitsConsumed = null,
           int? unitsEnabled = null,
           int? unitsSuspended = null,
           int? unitsWarning = null
           )
        {
            var context = root.Context.GraphApi;

            var data = new SubscribedSku
            {
                Id = Guid.NewGuid().ToString(),
                SkuId = skuId ?? Guid.NewGuid(),
                SkuPartNumber = skuPartNumber,
                ServicePlans = new List<ServicePlanInfo>(),
                ConsumedUnits = unitsConsumed ?? 0,
                PrepaidUnits = new LicenseUnitsDetail
                {
                    Enabled = unitsEnabled ?? 0,
                    Suspended = unitsSuspended ?? 0,
                    Warning = unitsWarning ?? 0
                }
            };

            context.SubscribedSkus.AddOrUpdate(data.Id, data, (id, existing) => { return data; });

            return data;
        }

        public static ServicePlanInfo WithServicePlan(this TestBuilderBase root,
           string subscribedSkuId,
           string name,
           Guid? servicePlanId=null
           )
        {
            var context = root.Context.GraphApi;

            if (context.SubscribedSkus.TryGetValue(subscribedSkuId, out var product))
            {
                var servicePlans = product.ServicePlans?.ToList() ?? new List<ServicePlanInfo>();

                var plan = new ServicePlanInfo
                {
                    ServicePlanId = servicePlanId ?? Guid.NewGuid(),
                    ServicePlanName = name
                };
                servicePlans.Add(plan);

                product.ServicePlans = servicePlans;

                return plan;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(subscribedSkuId));
            }
        }

        public static Group WithGroup(this TestBuilderBase root,
            string tenantId,
            string name,
            string licenseProcessingState = null
            )
        {
            var context = root.Context.GraphApi;

            var data = new Group
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = name,
                AssignedLicenses = new List<AssignedLicense>(),
                LicenseProcessingState = new LicenseProcessingState { State = licenseProcessingState}
            };

            context.Groups.AddOrUpdate(data.Id, data, (id, existing) => { return data; });

            return data;
        }
    }
}
