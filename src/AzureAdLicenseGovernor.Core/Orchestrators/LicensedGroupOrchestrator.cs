using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupOrchestrator
    {
        private readonly LicensedGroupRepository _licensedGroupRepository;
        private readonly DirectoryOrchestrator _directoryOrchestrator;

        public LicensedGroupOrchestrator(LicensedGroupRepository licensedGroupRepository,
            DirectoryOrchestrator directoryOrchestrator)
        {
            _licensedGroupRepository = licensedGroupRepository;
            _directoryOrchestrator = directoryOrchestrator;
        }

        public Task<ICollection<LicensedGroup>> Get()
        {
            var results = _licensedGroupRepository.Get();

            return results;
        }

        public Task<ICollection<LicensedGroup>> Get(string tenantId)
        {
            var results = _licensedGroupRepository.Get(tenantId);

            return results;
        }

        public Task<LicensedGroup> Get(string tenantId, string objectId)
        {
            var result = _licensedGroupRepository.Get(tenantId, objectId);

            return result;
        }

        public async Task Add(LicensedGroup toAdd)
        {
            await ValidateTenantId(toAdd);
            await _licensedGroupRepository.Save(toAdd);
        }

        public async Task Update(string tenantId, string objectId, LicensedGroup toUpdate)
        {
            toUpdate.ObjectId = objectId;
            toUpdate.TenantId = tenantId;

            await ValidateTenantId(toUpdate);

            await _licensedGroupRepository.Save(toUpdate);
        }

        public Task Delete(string tenantId, string objectId)
        {
            return _licensedGroupRepository.Delete(tenantId, objectId);
        }

        private async Task ValidateTenantId(LicensedGroup group)
        {
            var directory = await _directoryOrchestrator.GetById(group.TenantId);
            if(directory == null)
            {
                throw new ArgumentOutOfRangeException(nameof(group.TenantId),"Unsupported tenant");
            }
        }
    }
}
