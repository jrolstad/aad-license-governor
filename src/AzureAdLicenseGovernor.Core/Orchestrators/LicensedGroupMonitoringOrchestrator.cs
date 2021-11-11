using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class LicensedGroupMonitoringOrchestrator
    {
        private readonly LicensedGroupOrchestrator _licensedGroupOrchestrator;
        private readonly DirectoryOrchestrator _directoryOrchestrator;
        private readonly GroupService _groupService;
        private readonly LoggingService _logger;

        public LicensedGroupMonitoringOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            GroupService groupService,
            LoggingService logger)
        {
            _licensedGroupOrchestrator = licensedGroupOrchestrator;
            _directoryOrchestrator = directoryOrchestrator;
            _groupService = groupService;
            _logger = logger;
        }

        public async Task Monitor()
        {
            var directories = await _directoryOrchestrator.Get();

            var directoryTasks = directories
                .Select(Monitor);

            await Task.WhenAll(directoryTasks);
        }

        private Task Monitor(Directory directory)
        {
            var monitoringTasks = new List<Task>
            {
                MonitorLicensedGroups(directory),
                MonitorGroupsWithLicensingErrors(directory)
            };

            return Task.WhenAll(monitoringTasks);
        }

        private async Task MonitorLicensedGroups(Directory directory)
        {
            var groups = await _licensedGroupOrchestrator.Get(directory.TenantId);
            var groupTasks = groups
                .Where(g => g.IsMonitored)
                .Select(group => MonitorGroup(directory, group));

            await Task.WhenAll(groupTasks);
        }

        private async Task MonitorGroup(Directory directory, LicensedGroup group)
        {
            var groupData = await _groupService.Get(directory, group.ObjectId);
            LogGroupLicenseProcessingState(groupData);
        }

        private void LogGroupLicenseProcessingState(Group groupData)
        {
            var data = new Dictionary<string, object>
            {
                {"TenantId",groupData.TenantId },
                {"GroupId",groupData.ObjectId },
                {"LicenseProcessingState",groupData.LicenseProcessingState },
            };
            _logger.LogInfo("Group Monitor:Processing State", data);
        }

        private async Task MonitorGroupsWithLicensingErrors(Directory directory)
        {
            var groups = await _groupService.GetGroupsWithLicensingErrors(directory);

            LogGroupLicenseErrorSummary(directory, groups);

            foreach (var group in groups)
            {
                LogGroupLicensingErrors(group);
            }
        }

        private void LogGroupLicenseErrorSummary(Directory directory, ICollection<Group> groups)
        {
            var data = new Dictionary<string, object>
            {
                { "TenantId",directory.TenantId },
                { "Count", groups.Count }
            };
            _logger.LogInfo("Group Monitor:Licensing Error Summary", data);
        }

        private void LogGroupLicensingErrors(Group group)
        {
            var data = new Dictionary<string, object>
                {
                    {"TenantId",group.TenantId },
                    {"GroupId",group.ObjectId },
                    {"LicenseProcessingState",group.LicenseProcessingState },
                };
            _logger.LogInfo("Group Monitor:Licensing Errors", data);
        }
    }
}
