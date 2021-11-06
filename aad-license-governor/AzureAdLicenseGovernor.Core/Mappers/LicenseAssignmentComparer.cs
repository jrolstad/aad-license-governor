using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureAdLicenseGovernor.Core.Mappers
{
    public class LicenseAssignmentComparer
    {
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
                .ToList();

            var productsToRemove = actual?
                .Where(a => !expectedAssignmentsById.ContainsKey(a.ProductId))
                .Select(a => Guid.Parse(a.ProductId))
                .ToList();

            var existingProducts = actual?
                .Where(a => expectedAssignmentsById.ContainsKey(a.ProductId))
                .ToList();

            return new LicenseAssignmentComparisonResult
            {
                ToAdd = new List<LicenseAssignment>(),
                ToRemove = new List<LicenseAssignment>(),
                ToUpdate = new List<LicenseAssignment>()
            };
        }
    }
}
