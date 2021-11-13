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
        private readonly LicensedAssignmentMapper _licensedAssignmentMapper;
        private readonly ILoggingService _logger;

        public LicensedGroupGovernanceOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            LicensedProductService licensedProductService,
            GroupService groupService,
            LicenseAssignmentComparer licenseAssignmentComparer,
            LicensedAssignmentMapper licensedAssignmentMapper,
            ILoggingService logger)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _directoryOrchestrator = directoryOrchestrator;
            _licensedProductService = licensedProductService;
            _groupService = groupService;
            _licenseAssignmentComparer = licenseAssignmentComparer;
            _licensedAssignmentMapper = licensedAssignmentMapper;
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
            var productsByName = products.ToDictionary(p => p.Name,StringComparer.OrdinalIgnoreCase);

            var groups = await _licensedGroupOrchestrator.Get(directory.TenantId);
            var groupTasks = groups.Select(group => ApplyGroup(directory,group, productsById, productsByName));

            await Task.WhenAll(groupTasks);
        }

        private async Task ApplyGroup(Directory directory, 
            LicensedGroup licensedGroup, 
            Dictionary<string, Product> productsById,
            Dictionary<string, Product> productsByName)
        {
            
            var group = await _groupService.Get(directory, licensedGroup.ObjectId);

            var configuredProducts = _licensedAssignmentMapper.MapIds(licensedGroup.LicensedProducts, productsById, productsByName);

            var comparisonResult = _licenseAssignmentComparer.Compare(group.AssignedLicenses,
                configuredProducts,
                productsById);

            if (licensedGroup.Mode == ProductAssignmentMode.Enforce)
            {
                await ApplyChanges(directory, licensedGroup, comparisonResult);
            }

            LogChangeSummary(licensedGroup,comparisonResult);
        }

        private Task ApplyChanges(Directory directory, 
            LicensedGroup licensedGroup, 
            LicenseAssignmentComparisonResult comparisonResult)
        {
            var actionTasks = new List<Task>();

            if (comparisonResult.ToAdd.Any())
            {
                LogLicenseChanges("Add", licensedGroup, comparisonResult.ToAdd);
                var addTask = _groupService.AssignLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToAdd);
                actionTasks.Add(addTask);
            }

            if (comparisonResult.ToRemove.Any())
            {
                LogLicenseChanges("Remove", licensedGroup, comparisonResult.ToRemove);
                var removeTask = _groupService.RemoveLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToRemove);
                actionTasks.Add(removeTask);
            }

            if (comparisonResult.ToUpdate.Any())
            {
                LogLicenseChanges("Update", licensedGroup, comparisonResult.ToUpdate);
                var updateTask = _groupService.UpdateLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToUpdate);
                actionTasks.Add(updateTask);
            }

            return Task.WhenAll(actionTasks);
        }

        private void LogLicenseChanges(string action, LicensedGroup group, IEnumerable<LicenseAssignment> changes)
        {
            foreach(var assignment in changes)
            {
                var data = new Dictionary<string, string>
                {
                    {"TenantId",group.TenantId },
                    {"GroupId",group.ObjectId },
                    {"AssignmentMode",group.Mode.ToString() },
                    {"Action",action },
                    {"ProductId",assignment.ProductId },
                    {"DisabledServicePlans", string.Join(",",assignment.DisabledServicePlans ?? new List<string>()) },
                };

                _logger.LogInfo(LogMessages.LicenseGovernanceAssignmentChange, data);
            }
            
        }
        private void LogChangeSummary(LicensedGroup group, LicenseAssignmentComparisonResult comparison)
        {
            var data = new Dictionary<string, string>
                {
                    {"TenantId",group.TenantId },
                    {"GroupId",group.ObjectId },
                    {"AssignmentMode",group.Mode.ToString() },
                    {"Added",comparison.ToAdd.Count.ToString() },
                    {"Removed",comparison.ToRemove.Count.ToString() },
                    {"Updated",comparison.ToUpdate.Count.ToString() },
                };

            if (comparison.ToUpdate.Any() ||
                comparison.ToAdd.Any() ||
                comparison.ToRemove.Any())
            {
                _logger.LogInfo(LogMessages.LicenseGovernanceChangeSummary, data);
            }
            else
            {
                _logger.LogInfo(LogMessages.LicenseGovernanceNoChange, data);
            }
        }

        
    }
}
