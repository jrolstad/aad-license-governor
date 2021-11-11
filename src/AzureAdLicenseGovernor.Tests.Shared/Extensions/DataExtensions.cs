using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class DataExtensions
    {
        public static LicensedGroupData WithLicensedGroup(this TestBuilderBase root,
           string tenantId,
           string objectId,
           ProductAssignmentMode mode = ProductAssignmentMode.Enforce
        )
        {
            var data = new LicensedGroupData
            {
                Id = $"{tenantId}|{objectId}",
                TenantId = tenantId ?? Guid.NewGuid().ToString(),
                ObjectId = objectId,
                Mode = mode,
                LicensedProducts = new List<LicensedProductAssignmentData>()
            };

            root.Context.Data.Groups.AddOrUpdate(data.Id, data, (id, existing) => { return data; });

            return data;
        }

        public static LicensedProductAssignmentData WithProductAssignment(this TestBuilderBase root,
           string tenantId,
           string objectId,
           string productId,
           params string[] enabledServicePlans)
        {
            var id = $"{tenantId}|{objectId}";
            if (root.Context.Data.Groups.TryGetValue(id,out var group))
            {
                var existing = group.LicensedProducts.FirstOrDefault(p => p.Id == productId);
                if(existing!=null)
                {
                    existing.EnabledServicePlans = enabledServicePlans.ToList();
                    return existing;
                }

                var assignment = new LicensedProductAssignmentData
                {
                    Id = productId,
                    EnabledServicePlans = enabledServicePlans.ToList()
                };
                group.LicensedProducts.Add(assignment);

                return assignment;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(objectId));
            }
        }

        public static DirectoryData WithDirectory(this TestBuilderBase root,
            string name,
            string tenantId = null,
            string domain = null,
            string graphUrl = "https://graph.microsoft.com",
            string portalUrl = "https://portal.azure.com",
            DirectoryClientType clientType = DirectoryClientType.Application,
            string clientId = "client-id",
            bool isDefault = true
        )
        {
            var data = new DirectoryData
            {
                Id = Guid.NewGuid().ToString(),
                TenantId = tenantId ?? Guid.NewGuid().ToString(),
                Area = CosmosConfiguration.DefaultPartitionKey,
                Name = name,
                Domain = domain ?? Guid.NewGuid().ToString(),
                GraphUrl = graphUrl,
                PortalUrl = portalUrl,
                ClientType = clientType,
                ClientId = clientId,
                IsDefault = isDefault,
                Monitoring = new DirectoryMonitoring
                {
                    TrackGroupLicenseAssignmentFailures = false,
                    TrackProductChanges = false,
                    TrackProductUsage = false
                }

            };

            root.Context.Data.Directories.AddOrUpdate(data.Id, data, (id, existing) => { return data; });

            return data;
        }

        public static void WithProductSnapshot(this TestBuilderBase testBuilder,
            string tenantId,
            string productId,
            string productName)
        {
            if (testBuilder.Context.Data.ProductSnapshots.TryGetValue(tenantId, out var existing))
            {
                var productData = new ProductData
                {
                    Id = productId,
                    TenantId = tenantId,
                    Name = productName
                };
                existing.Products.Add(productData);

            }
            else
            {
                var newSnapshot = new ProductDataSnapshot
                {
                    Id = tenantId,
                    TenantId = tenantId,
                    Products = new List<ProductData>
                {
                    new ProductData
                    {
                        Id = productId,
                        TenantId = tenantId,
                        Name = productName
                    }
                }
                };

                testBuilder.Context.Data.ProductSnapshots.AddOrUpdate(tenantId, newSnapshot, (id, existing) => newSnapshot);
            }
        }

        public static void WithProductSnapshotServicePlan(this TestBuilderBase testBuilder,
            string tenantId,
            string productName,
            string servicePlanId,
            string servicePlanName)
        {
            if(testBuilder.Context.Data.ProductSnapshots.TryGetValue(tenantId,out var snapshot))
            {
                var product = snapshot.Products.FirstOrDefault(p => p.Name == productName);
                if(product!=null)
                {
                    product.ServicePlans = product.ServicePlans ?? new List<ServicePlan>();
                    product.ServicePlans.Add(new ServicePlan 
                    { 
                        Id = servicePlanId,
                        Name = servicePlanName
                    });
                }
            }
        }
    }
}
