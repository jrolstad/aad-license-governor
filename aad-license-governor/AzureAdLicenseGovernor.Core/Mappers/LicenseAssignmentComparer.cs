using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicenseAssignmentComparer
    {
        private readonly LicensedAssignmentMapper _licensedAssignmentMapper;

        public LicenseAssignmentComparer(LicensedAssignmentMapper licensedAssignmentMapper)
        {
            _licensedAssignmentMapper = licensedAssignmentMapper;
        }

        public LicenseAssignmentComparisonResult Compare(ICollection<LicenseAssignment> actual,
            ICollection<ProductAssignment> expected,
            Dictionary<string, Product> availableProducts)
        {
            var existingAssignmentsById = actual?
                .ToDictionary(a => a.ProductId) ?? new Dictionary<string, LicenseAssignment>();

            var expectedAssignmentsById = expected?
                .ToDictionary(p => p.Id) ?? new Dictionary<string, ProductAssignment>();

            var productsToAdd = expected?
                .Where(p => !existingAssignmentsById.ContainsKey(p.Id))
                .Select(p=>_licensedAssignmentMapper.Map(p,GetProductServicePlans(p.Id,availableProducts)))
                .ToList();

            var productsToRemove = actual?
                .Where(a => !expectedAssignmentsById.ContainsKey(a.ProductId))
                .ToList();

            var existingProducts = actual?
                .Where(a => expectedAssignmentsById.ContainsKey(a.ProductId))
                .ToList();

            return new LicenseAssignmentComparisonResult
            {
                ToAdd = productsToAdd,
                ToRemove = productsToRemove,
                ToUpdate = new List<LicenseAssignment>()
            };
        }

        private IEnumerable<string> GetProductServicePlans(string productId, Dictionary<string, Product> availableProducts)
        {
            if(availableProducts.TryGetValue(productId,out Product product))
            {
                return product?
                    .ServicePlans?
                    .Select(s => s.Id) ?? new string[0];
            }

            return new string[0];
        }
    }
}
