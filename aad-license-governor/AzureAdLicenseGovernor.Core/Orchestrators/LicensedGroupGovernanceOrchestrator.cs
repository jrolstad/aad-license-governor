using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Services;
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
        private readonly LicensedAssignmentMapper _licensedAssignmentMapper;
        private readonly LicenseAssignmentComparer _licenseAssignmentComparer;

        public LicensedGroupGovernanceOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            LicensedProductService licensedProductService,
            GroupService groupService,
            LicensedAssignmentMapper licensedAssignmentMapper,
            LicenseAssignmentComparer licenseAssignmentComparer)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _directoryOrchestrator = directoryOrchestrator;
            _licensedProductService = licensedProductService;
            _groupService = groupService;
            _licensedAssignmentMapper = licensedAssignmentMapper;
            _licenseAssignmentComparer = licenseAssignmentComparer;
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

            var actionTasks = new List<Task>();
            if(comparisonResult.ToAdd.Any())
            {
                var addTask =_groupService.AssignedLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToAdd);
                actionTasks.Add(addTask);
            }
            if(comparisonResult.ToRemove.Any())
            {
                var removeTask = _groupService.RemoveLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToRemove);
                actionTasks.Add(removeTask);
            }

            if(comparisonResult.ToUpdate.Any())
            {
                var updateTask = _groupService.UpdateLicenses(directory, licensedGroup.ObjectId, comparisonResult.ToRemove);
                actionTasks.Add(updateTask);
            }

            await Task.WhenAll(actionTasks);
        }
 
    }
}
