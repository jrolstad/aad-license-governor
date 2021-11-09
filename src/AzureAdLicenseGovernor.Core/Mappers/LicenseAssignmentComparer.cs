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

            var productsToUpdate = GetProductsWithServicePlanChanges(actual, expectedAssignmentsById, availableProducts)
                .ToList();

            return new LicenseAssignmentComparisonResult
            {
                ToAdd = productsToAdd,
                ToRemove = productsToRemove,
                ToUpdate = productsToUpdate
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

        private IEnumerable<LicenseAssignment> GetProductsWithServicePlanChanges(ICollection<LicenseAssignment> existing,
            Dictionary<string, ProductAssignment> expected,
            Dictionary<string, Product> availableProducts)
        {
            var existingProducts = existing?
               .Where(a => expected.ContainsKey(a.ProductId))
               .ToList() ?? new List<LicenseAssignment>();

            foreach (var product in existingProducts)
            {
                var expectedLicenseAssignment = GetExpectedLicenseAssignment(expected, availableProducts, product);
                var existingLicenseAssignment = GetExistingLicenseAssignment(existing, product);

                if (!LicenseAssignmentsAreSame(expectedLicenseAssignment, existingLicenseAssignment))
                {
                    yield return expectedLicenseAssignment;
                }
            }

        }

        private LicenseAssignment GetExpectedLicenseAssignment(Dictionary<string, ProductAssignment> expected, Dictionary<string, Product> availableProducts, LicenseAssignment product)
        {
            var expectedAssignment = expected[product.ProductId];
            var servicePlans = GetProductServicePlans(product.ProductId, availableProducts);
            var expectedLicenseAssignment = _licensedAssignmentMapper.Map(expectedAssignment, servicePlans);
            return expectedLicenseAssignment;
        }

        private static LicenseAssignment GetExistingLicenseAssignment(ICollection<LicenseAssignment> existing, LicenseAssignment product)
        {
            return existing.FirstOrDefault(a => string.Equals(a.ProductId, product.ProductId, StringComparison.OrdinalIgnoreCase))
                ?? new LicenseAssignment { ProductId = product.ProductId, DisabledServicePlans = new List<string>() };
        }

        private static bool LicenseAssignmentsAreSame(LicenseAssignment expectedLicenseAssignment, LicenseAssignment existingLicenseAssignment)
        {
            return string.Equals(expectedLicenseAssignment.ProductId, existingLicenseAssignment.ProductId, StringComparison.OrdinalIgnoreCase)
                && expectedLicenseAssignment.DisabledServicePlans.SequenceEqual(existingLicenseAssignment.DisabledServicePlans);
        }
    }
}
