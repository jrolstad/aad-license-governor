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
    public class GroupService
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly GroupMapper _mapper;
        private readonly LicensedAssignmentMapper _licensedAssignmentMapper;

        public GroupService(IGraphClientFactory graphClientFactory, 
            GroupMapper mapper,
            LicensedAssignmentMapper licensedAssignmentMapper)
        {
            _graphClientFactory = graphClientFactory;
            _mapper = mapper;
            _licensedAssignmentMapper = licensedAssignmentMapper;
        }

        public async Task<Group> Get(Directory directory, string id)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var data =await client.Groups[id]
                .Request()
                .Select("id,displayName,assignedLicenses,licenseprocessingstate")
                .GetAsync();

            var result = _mapper.Map(directory, data);

            return result;
        }

        public async Task<ICollection<Group>> GetGroupsWithLicensingErrors(Directory directory)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var data = await client.Groups
                .Request()
                .Filter("hasMembersWithLicenseErrors eq true")
                .Select("id,displayName,assignedLicenses,licenseprocessingstate")
                .GetAsync();

            var result = data.Select(d=>_mapper.Map(directory, d))
                .ToList();

            return result;
        }

        public async Task AssignedLicenses(Directory directory, 
            string id,
            ICollection<LicenseAssignment> toAdd)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var assignmentsToAdd = toAdd.Select(_licensedAssignmentMapper.Map);

            await client.Groups[id]
                .AssignLicense(assignmentsToAdd, new List<Guid>())
                .Request()
                .PostAsync();
        }

        public async Task RemoveLicenses(Directory directory,
            string id,
            ICollection<LicenseAssignment> toRemove)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var assignmentsToRemove = toRemove
                .Select(a => Guid.Parse(a.ProductId));

            await client.Groups[id]
                .AssignLicense(new List<Microsoft.Graph.AssignedLicense>(), assignmentsToRemove)
                .Request()
                .PostAsync();
        }

        public async Task UpdateLicenses(Directory directory,
            string id,
            ICollection<LicenseAssignment> toUpdate)
        {
            var client = await _graphClientFactory.CreateAsync(directory);

            var assignmentsToRemove = toUpdate
                .Select(a => Guid.Parse(a.ProductId));
            var assignmentsToAdd = toUpdate.Select(_licensedAssignmentMapper.Map);

            await client.Groups[id]
                .AssignLicense(assignmentsToAdd,assignmentsToRemove)
                .Request()
                .PostAsync();
        }
    }
}
