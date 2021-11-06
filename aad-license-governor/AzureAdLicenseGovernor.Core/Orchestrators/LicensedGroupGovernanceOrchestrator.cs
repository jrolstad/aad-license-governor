using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupGovernanceOrchestrator
    {
        private readonly LicensedGroupOrchestrator _licensedGroupOrchestrator;
        private readonly DirectoryOrchestrator _directoryOrchestrator;
        private readonly LicensedProductService _licensedProductService;
        private readonly GroupService _groupService;
        private readonly LicenseAssignmentComparer _licenseAssignmentComparer;
        private readonly ILogger<LicensedGroupGovernanceOrchestrator> _logger;

        public LicensedGroupGovernanceOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            LicensedProductService licensedProductService,
            GroupService groupService,
            LicenseAssignmentComparer licenseAssignmentComparer,
            ILogger<LicensedGroupGovernanceOrchestrator> logger)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _directoryOrchestrator = directoryOrchestrator;
            _licensedProductService = licensedProductService;
            _groupService = groupService;
            _licenseAssignmentComparer = licenseAssignmentComparer;
            _logger = logger;
        }

        public async Task Apply()
        {
            var directories = await _directoryOrchestrator.Get();

            var directoryTasks = directories
                .Select(Apply);

            await Task.WhenAll(directoryTasks);
        }

        private async Task Apply(Directory directory)
        {
            var products = await _licensedProductService.Get(directory);
            var productsById = products.ToDictionary(p => p.Id,StringComparer.OrdinalIgnoreCase);

            var groups = await _licensedGroupOrchestrator.Get(directory.TenantId);
            var groupTasks = groups.Select(group => ApplyGroup(directory,group, productsById));

            await Task.WhenAll(groupTasks);
        }

        private async Task ApplyGroup(Directory directory, 
            LicensedGroup licensedGroup, 
            Dictionary<string, Product> productsById)
        {
            var group = await _groupService.Get(directory, licensedGroup.ObjectId);

            var comparisonResult = _licenseAssignmentComparer.Compare(group.AssignedLicenses,
                licensedGroup.LicensedProducts,
                productsById);

            if (licensedGroup.Mode == ProductAssignmentMode.Enforce)
            {
                await ApplyChanges(directory, licensedGroup, comparisonResult);
            }

            LogChanges(licensedGroup,comparisonResult);
        }

        private Task ApplyChanges(Directory directory, LicensedGroup licensedGroup, LicenseAssignmentComparisonResult comparisonResult)
        {
            var actionTasks = new List<Task>();

            if (comparisonResult.ToAdd.Any())
            {
                var addTask = _groupService.AssignedLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToAdd);
                actionTasks.Add(addTask);
            }

            if (comparisonResult.ToRemove.Any())
            {
                var removeTask = _groupService.RemoveLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToRemove);
                actionTasks.Add(removeTask);
            }

            if (comparisonResult.ToUpdate.Any())
            {
                var updateTask = _groupService.UpdateLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToRemove);
                actionTasks.Add(updateTask);
            }

            return Task.WhenAll(actionTasks);
        }

        private void LogChanges(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            LogAddedProducts(group, comparison);
            LogRemovedProducts(group, comparison);
            LogUpdatedProducts(group, comparison);

            LogIfNoChanges(group,comparison);
        }

        private void LogAddedProducts(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            if (comparison.ToAdd.Any())
            {
                _logger.LogInformation("Licensed products added to group");
            }
            else
            {
                _logger.LogInformation("No licensed products added to group");
            }
        }

        private void LogRemovedProducts(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            if (comparison.ToRemove.Any())
            {
                _logger.LogInformation("Licensed products removed from group");
            }
            else
            {
                _logger.LogInformation("No licensed products removed from group");
            }
        }

        private void LogUpdatedProducts(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            if (comparison.ToUpdate.Any())
            {
                _logger.LogInformation("Licensed products updated for group");
            }
            else
            {
                _logger.LogInformation("No licensed products updated for group");
            }
        }

        private void LogIfNoChanges(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            if (!comparison.ToAdd.Any() &&
                            !comparison.ToRemove.Any() &&
                            !comparison.ToUpdate.Any())
            {
                _logger.LogInformation("No licensing changes for group");
            }
        }
    }
}
