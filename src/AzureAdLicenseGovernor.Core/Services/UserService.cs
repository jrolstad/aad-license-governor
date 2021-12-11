using AzureAdLicenseGovernor.Core.Extensions;
using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class UserService
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly UserMapper _userMapper;

        public UserService(IGraphClientFactory graphClientFactory, UserMapper userMapper)
        {
            _graphClientFactory = graphClientFactory;
            _userMapper = userMapper;
        }

        public async Task<ICollection<Models.User>> GetUserLicenseAssignmentStates(Models.Directory directory, IEnumerable<string> userIds)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var requests = userIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(id => client.Users[id]
                                    .Request()
                                    .Select("id,userPrincipalName,licenseAssignmentStates")
                                    );

            var userData = await client.ExecuteBatch<Microsoft.Graph.User>(requests);

            var result = userData
                .AsParallel()
                .Select(u => _userMapper.Map(directory, u))
                .ToList();

            return result;
        }
    }
}
