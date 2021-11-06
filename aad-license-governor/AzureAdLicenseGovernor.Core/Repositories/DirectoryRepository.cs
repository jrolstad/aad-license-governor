using AzureAdLicenseGovernor.Core.Configuration;
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
    public class DirectoryRepository
    {
        private readonly CosmosDbService _cosmosDbService;
        private readonly DirectoryMapper _directoryMapper;

        public DirectoryRepository(CosmosDbService cosmosDbService,
            DirectoryMapper directoryMapper)
        {
            _cosmosDbService = cosmosDbService;
            _directoryMapper = directoryMapper;
        }

        public Task<ICollection<Directory>> Get()
        {
            var query = _cosmosDbService
                .Query<DirectoryData>(CosmosConfiguration.Containers.Directories);

            var result = _cosmosDbService.ExecuteRead(query, _directoryMapper.Map);

            return result;
        }

        public async Task<Directory> GetById(string id)
        {
            var query = _cosmosDbService
               .Query<DirectoryData>(CosmosConfiguration.Containers.Directories)
               .Where(d => d.Id == id || d.TenantId == id || d.Domain == id);

            var data = await _cosmosDbService.ExecuteRead(query, _directoryMapper.Map);

            var result = data.FirstOrDefault();

            return result;
        }

        public async Task<Directory> Save(Directory toSave)
        {
            var data = _directoryMapper.Map(toSave);
            var result = await _cosmosDbService.Save(data,
                CosmosConfiguration.Containers.Directories);

            var savedData = _directoryMapper.Map(result);
            return savedData;
        }

        public Task Delete(string id)
        {
            var result = _cosmosDbService.Delete<DirectoryData>(id,
                CosmosConfiguration.Containers.Directories,
                CosmosConfiguration.DefaultPartitionKey);

            return result;
        }
    }
}
