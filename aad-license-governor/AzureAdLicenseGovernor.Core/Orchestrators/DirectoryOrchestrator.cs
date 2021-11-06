using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class DirectoryOrchestrator
    {
        private readonly DirectoryRepository _directoryRepository;

        public DirectoryOrchestrator(DirectoryRepository directoryRepository)
        {
            _directoryRepository = directoryRepository;
        }

        public Task<ICollection<Directory>> Get()
        {
            var result = _directoryRepository.Get();

            return result;
        }

        public Task<Directory> GetById(string id)
        {
            var result = _directoryRepository.GetById(id);

            return result;
        }

        public Task<Directory> Add(Directory toAdd)
        {
            toAdd.Id = Guid.NewGuid().ToString();
            var item = _directoryRepository.Save(toAdd);

            return item;
        }

        public async Task<Directory> Update(string id, Directory toUpdate)
        {
            var existing = await GetById(id);
            if (string.IsNullOrEmpty(existing?.Id)) return null;

            toUpdate.Id = id;
            var item = await _directoryRepository.Save(toUpdate);

            return item;
        }

        public async Task Delete(string id)
        {
            var directory = await GetById(id);

            if (directory == null) return;
            await _directoryRepository.Delete(directory.Id);
        }
    }
}
