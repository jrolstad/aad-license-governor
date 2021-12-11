using AzureAdLicenseGovernor.Core.Factories;
using AzureAdLicenseGovernor.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Text;
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
            return new List<Models.User>();
        }
    }
}
