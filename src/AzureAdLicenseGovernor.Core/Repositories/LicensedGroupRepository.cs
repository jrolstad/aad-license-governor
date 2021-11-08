using AzureAdLicenseGovernor.Core.Configuration.Cosmos;
using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Models.Data;
using AzureAdLicenseGovernor.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Repositories
{
    public class LicensedGroupRepository
    {
        private readonly LicensedGroupMapper _licensedGroupMapper;
        private readonly CosmosDbService _cosmosDbService;

        public LicensedGroupRepository(LicensedGroupMapper licensedGroupMapper,
            CosmosDbService cosmosDbService)
        {
            _licensedGroupMapper = licensedGroupMapper;
            _cosmosDbService = cosmosDbService;
        }

        public async Task<ICollection<LicensedGroup>> Get()
        {
            var query = _cosmosDbService.Query<LicensedGroupData>(CosmosConfiguration.Containers.LicensedGroups);

            var data = await _cosmosDbService.ExecuteRead(query,_licensedGroupMapper.Map);

            return data.ToList();
        }

        public async Task<ICollection<LicensedGroup>> Get(string tenantId)
        {
            var query = _cosmosDbService.Query<LicensedGroupData>(CosmosConfiguration.Containers.LicensedGroups)
                .Where(g => g.TenantId == tenantId);

            var data = await _cosmosDbService.ExecuteRead(query, _licensedGroupMapper.Map);

            return data.ToList();
        }

        public async Task<LicensedGroup> Get(string tenantId, string objectId)
        {
            var query = _cosmosDbService
               .Query<LicensedGroupData>(CosmosConfiguration.Containers.LicensedGroups)
               .Where(d => d.ObjectId == objectId && d.TenantId == tenantId);

            var data = await _cosmosDbService.ExecuteRead(query, _licensedGroupMapper.Map);

            var result = data.FirstOrDefault();

            return result;
        }

        public async Task<LicensedGroup> Save(LicensedGroup toSave)
        {
            var data = _licensedGroupMapper.Map(toSave);
            var result = await _cosmosDbService.Save(data,
                CosmosConfiguration.Containers.LicensedGroups);

            var savedData = _licensedGroupMapper.Map(result);
            return savedData;
        }

        public Task Delete(string tenantId, string objectId)
        {
            var id = _licensedGroupMapper.GetId(tenantId, objectId);

            var result = _cosmosDbService.Delete<LicensedGroupData>(id,
                CosmosConfiguration.Containers.LicensedGroups,
                tenantId);

            return result;
        }
    }
}
