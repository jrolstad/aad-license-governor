using AzureAdLicenseGovernor.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class ProductComparer
    {
        public TenantProductComparisonResult Compare(ICollection<Product> actual,ICollection<Product> expected)
        {
            var expectedById = expected.ToDictionary(p => p.Id);
            var actualById = actual.ToDictionary(p => p.Id);

            var productsToAdd = actual?
                .Where(p => !expectedById.ContainsKey(p.Id))
                .ToList() ?? new List<Product>();

            var productsToRemove = expected?
                .Where(a => !actualById.ContainsKey(a.Id))
                .ToList() ?? new List<Product>();

            var productsToUpdate = GetProductsWithServicePlanChanges(actual, expectedById)
                .ToList() ?? new List<ProductComparisonResult>();

            return new TenantProductComparisonResult
            {
                Added = productsToAdd,
                Removed = productsToRemove,
                Updated = productsToUpdate
            };
        }

        private IEnumerable<ProductComparisonResult> GetProductsWithServicePlanChanges(ICollection<Product> actualSet, Dictionary<string, Product> expectedById)
        {
            foreach(var actual in actualSet)
            {
                if(expectedById.TryGetValue(actual.Id,out var expected))
                {
                    var actualServicePlansById = actual.ServicePlans?.ToDictionary(a => a.Id);
                    var expectedServicePlansById = expected.ServicePlans?.ToDictionary(a => a.Id);

                    var servicePlansToAdd = actual.ServicePlans?
                       .Where(p => !expectedServicePlansById.ContainsKey(p.Id))
                       .ToList() ?? new List<ServicePlan>();

                    var servicePlansToRemove = expected.ServicePlans?
                        .Where(a => !actualServicePlansById.ContainsKey(a.Id))
                        .ToList() ?? new List<ServicePlan>();

                    if(servicePlansToAdd.Any() || servicePlansToRemove.Any())
                    {
                        yield return new ProductComparisonResult
                        {
                            Product = actual,
                            ServicePlans = new ServicePlanComparisonResult
                            {
                                Added = servicePlansToAdd,
                                Removed = servicePlansToRemove
                            }
                        };
                    }
                }
            }
            
        }
    }

}
