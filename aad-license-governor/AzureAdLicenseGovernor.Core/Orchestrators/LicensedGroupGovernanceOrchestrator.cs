using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupGovernanceOrchestrator
    {
        private readonly LicensedGroupOrchestrator _licensedGroupOrchestrator;
        private readonly DirectoryOrchestrator _directoryOrchestrator;
        private readonly LicensedProductService _licensedProductService;
        private readonly GroupService _groupService;

        public LicensedGroupGovernanceOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            LicensedProductService licensedProductService,
            GroupService groupService)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _directoryOrchestrator = directoryOrchestrator;
            _licensedProductService = licensedProductService;
            _groupService = groupService;
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

            var productsById = products.ToDictionary(p => p.Id);

            var groups = await _licensedGroupOrchestrator.Get(directory.TenantId);

            var groupTasks = groups.Select(group => ApplyGroup(directory,group, productsById));

            await Task.WhenAll(groupTasks);
           
        }

        private async Task ApplyGroup(Directory directory, 
            LicensedGroup licensedGroup, 
            Dictionary<string, LicensedProduct> productsById)
        {
            var group = await _groupService.Get(directory, licensedGroup.ObjectId);

            
        }

        
    }
}
