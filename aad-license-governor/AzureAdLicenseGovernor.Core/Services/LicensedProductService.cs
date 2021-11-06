using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class LicensedProductService
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly LicensedProductMapper _mapper;

        public LicensedProductService(IGraphClientFactory graphClientFactory,
            LicensedProductMapper mapper)
        {
            _graphClientFactory = graphClientFactory;
            _mapper = mapper;
        }

        public async Task<List<Product>> Get(Models.Directory directory)
        {
            var data = await GetSubscribedSkus(directory);
            var result = data.Select(_mapper.Map).ToList();

            return result;
        }

        private async Task<List<Microsoft.Graph.SubscribedSku>> GetSubscribedSkus(Directory directory)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var request = await client.SubscribedSkus
                .Request()
                .GetAsync();

            var data = new List<Microsoft.Graph.SubscribedSku>();
            while (request != null)
            {
                data.AddRange(request);

                if (request.NextPageRequest == null) break;
                request = await request.NextPageRequest.GetAsync();
            };

            return data;
        }
    }
}
