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
    public class ProductRepository
    {
        private readonly CosmosDbService _cosmosDbService;
        private readonly ProductMapper _productMapper;

        public ProductRepository(CosmosDbService cosmosDbService, ProductMapper productMapper)
        {
            _cosmosDbService = cosmosDbService;
            _productMapper = productMapper;
        }

        public async Task<ICollection<Product>> GetSnapshot(string tenantId)
        {
            var query = _cosmosDbService.Query<ProductDataSnapshot>(CosmosConfiguration.Containers.ProductSnapshots)
                .Where(g => g.TenantId == tenantId);

            var data = await _cosmosDbService.ExecuteRead(query, _productMapper.Map);

            return data.SelectMany(p => p).ToList();
        }

        public Task SaveSnapshot(string tenantId, List<Product> current)
        {
            var snapshot = _productMapper.Map(tenantId, current);

            return _cosmosDbService.Save(snapshot, CosmosConfiguration.Containers.ProductSnapshots);
        }
    }
}
