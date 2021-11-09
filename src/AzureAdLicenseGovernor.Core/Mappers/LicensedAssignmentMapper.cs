using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicensedAssignmentMapper
    {
        public Models.LicenseAssignment Map(Microsoft.Graph.AssignedLicense toMap)
        {
            return new Models.LicenseAssignment
            {
                ProductId = toMap?.SkuId.ToString(),
                DisabledServicePlans = toMap?.DisabledPlans?.Select(s => s.ToString())?.ToList()
            };
        }

        public Microsoft.Graph.AssignedLicense Map(Models.LicenseAssignment toMap)
        {
            return new Microsoft.Graph.AssignedLicense
            {
                SkuId = Guid.Parse(toMap?.ProductId),
                DisabledPlans = toMap?.DisabledServicePlans?.Select(Guid.Parse) ?? new List<Guid>()
            };
        }

        public Models.LicenseAssignment Map(Models.ProductAssignment toMap, IEnumerable<string> servicePlansForProduct)
        {
            var disabledServicePlans = servicePlansForProduct
                .Except(toMap.EnabledServicePlans,StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new Models.LicenseAssignment
            {
                ProductId = toMap.Id,
                DisabledServicePlans = disabledServicePlans
            };
        }

        public List<ProductAssignment> MapIds(List<ProductAssignment> licensedProducts, 
            Dictionary<string, Product> productsById, 
            Dictionary<string, Product> productsByName)
        {
            var result = licensedProducts
                .Select(assignment => new { 
                    Assignment = assignment, 
                    Product = GetProduct(assignment.Id, productsById, productsByName) 
                })
                .Where(resolved => resolved.Product != null)
                .Select(resolved => new ProductAssignment 
                { 
                    Id = resolved.Product.Id,
                    EnabledServicePlans = ResolveServicePlans(resolved.Assignment.EnabledServicePlans, resolved.Product)
                })
                .ToList();

            return result;
        }

        private Product GetProduct(string id, 
            Dictionary<string, Product> productsById,
            Dictionary<string, Product> productsByName)
        {
            if (productsById.TryGetValue(id, out var itemById)) return itemById;
            if (productsByName.TryGetValue(id, out var itemByName)) return itemByName;

            return null;
        }

        private ServicePlan GetServicePlan(string id,
            Dictionary<string, ServicePlan> servicePlansById,
            Dictionary<string, ServicePlan> servicePlansByName)
        {
            if (servicePlansById.TryGetValue(id, out var itemById)) return itemById;
            if (servicePlansByName.TryGetValue(id, out var itemByName)) return itemByName;

            return null;
        }

        private List<string> ResolveServicePlans(IEnumerable<string> servicePlans,
            Product product)
        {
            var servicePlansById = product.ServicePlans.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase);
            var servicePlansByName = product.ServicePlans.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

            var resolvedServicePlans = servicePlans
                .Select(s => GetServicePlan(s, servicePlansById, servicePlansByName))
                .Where(s => s != null)
                .Select(s => s.Id)
                .ToList();

            return resolvedServicePlans;
        }
    }

    
}
