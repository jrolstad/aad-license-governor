using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class GroupService
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly GroupMapper _mapper;

        public GroupService(IGraphClientFactory graphClientFactory, GroupMapper mapper)
        {
            _graphClientFactory = graphClientFactory;
            _mapper = mapper;
        }

        public async Task<Group> Get(Directory directory, string id)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var data =await client.Groups[id]
                .Request()
                .Select("id,displayName,assignedLicenses")
                .GetAsync();

            var result = _mapper.Map(directory, data);

            return result;
        }
    }
}
