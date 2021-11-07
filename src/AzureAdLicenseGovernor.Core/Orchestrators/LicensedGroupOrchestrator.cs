using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupOrchestrator
    {
        private readonly LicensedGroupRepository _licensedGroupRepository;

        public LicensedGroupOrchestrator(LicensedGroupRepository licensedGroupRepository)
        {
            _licensedGroupRepository = licensedGroupRepository;
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

        public Task Add(LicensedGroup toAdd)
        {
            return _licensedGroupRepository.Save(toAdd);
        }

        public Task Update(string tenantId, string objectId, LicensedGroup toUpdate)
        {
            toUpdate.ObjectId = objectId;
            toUpdate.TenantId = tenantId;

            return _licensedGroupRepository.Save(toUpdate);
        }

        public Task Delete(string tenantId, string objectId)
        {
            return _licensedGroupRepository.Delete(tenantId, objectId);
        }
    }
}
