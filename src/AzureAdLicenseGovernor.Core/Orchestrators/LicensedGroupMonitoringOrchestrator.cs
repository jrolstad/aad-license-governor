using AzureAdLicenseGovernor.Core.Extensions;
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
        private readonly ILoggingService _logger;

        public LicensedGroupMonitoringOrchestrator(LicensedGroupOrchestrator licensedGroupOrchestrator,
            DirectoryOrchestrator directoryOrchestrator,
            GroupService groupService,
            ILoggingService logger)
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
                .Where(g => g.TrackLicenseProcessingState)
                .Select(group => MonitorGroup(directory, group));

            await Task.WhenAll(groupTasks);
        }

        private async Task MonitorGroup(Directory directory, LicensedGroup group)
        {
            var groupData = await _groupService.Get(directory, group.ObjectId);
            LogGroupLicenseProcessingState(groupData, group);
        }

        private void LogGroupLicenseProcessingState(Group groupData, LicensedGroup group)
        {
            var data = new Dictionary<string, string>
            {
                {"TenantId",groupData.TenantId },
                {"GroupId",groupData.ObjectId },
                {"DisplayName",groupData.DisplayName },
                {"TrackLicenseProcessingState",group.TrackLicenseProcessingState.ToString() },
                {"AssignmentMode",group.Mode.ToString() },
                {"LicenseProcessingState",groupData.LicenseProcessingState },
            };
            _logger.LogInfo(LogMessages.GroupMonitorProcessingState, data);
        }

        private async Task MonitorGroupsWithLicensingErrors(Directory directory)
        {
            if (!directory.Monitoring.TrackGroupLicenseAssignmentFailures) return;

            var groups = await _groupService.GetGroupsWithLicensingErrors(directory);

            LogGroupLicenseErrorSummary(directory, groups);
            Parallel.ForEach(groups, LogGroupLicensingErrors);

            await MonitorUsersWithLicensingErrors(directory, groups);
        }

        private void LogGroupLicenseErrorSummary(Directory directory, ICollection<Group> groups)
        {
            var data = new Dictionary<string, string>
            {
                { "TenantId",directory.TenantId },
            };
            _logger.LogMetric(LogMessages.GroupMonitorLicensingErrorEvent,
                "GroupCount",
                groups.Count,
                data);
        }

        private void LogGroupLicensingErrors(Group groupData)
        {
            var data = new Dictionary<string, string>
                {
                    {"TenantId",groupData.TenantId },
                    {"GroupId",groupData.ObjectId },
                    {"DisplayName",groupData.DisplayName },
                    {"LicenseProcessingState",groupData.LicenseProcessingState },
                };
            _logger.LogInfo(LogMessages.GroupMonitorLicensingErrors, data);
        }

        private Task MonitorUsersWithLicensingErrors(Directory directory, IEnumerable<Group> groupsWithErrors)
        {
            if (directory.Monitoring?.TrackUserLicenseAssignmentFailures == false) return Task.CompletedTask;

            var groupMonitoringTasks = groupsWithErrors
                .Select(g => MonitorUsersWithLicensingErrors(directory, g));

            return Task.WhenAll(groupMonitoringTasks);
        }

        private async Task MonitorUsersWithLicensingErrors(Directory directory, Group group)
        {
            var users = await _groupService.GetUsersWithLicensingErrors(directory, group.ObjectId);

            LogUserLicenseErrorSummary(directory, group, users);
            Parallel.ForEach(users, user => LogUserLicensingErrors(group, user));
        }

        private void LogUserLicenseErrorSummary(Directory directory, Group group, ICollection<User> groups)
        {
            var data = new Dictionary<string, string>
            {
                {"TenantId",directory.TenantId },
                {"GroupId",group.ObjectId },
                {"DisplayName",group.DisplayName },
            };

            _logger.LogMetric(LogMessages.GroupMonitorUserLicensingErrorEvent,
                "UserCount",
                groups.Count,
                data);
        }

        private void LogUserLicensingErrors(Group group, User user)
        {
            var data = new Dictionary<string, string>
                {
                    {"TenantId",group?.TenantId },
                    {"GroupId",group?.ObjectId },
                    {"GroupDisplayName",group?.DisplayName },
                    {"UserId",user?.ObjectId },
                    {"UserPrincipalName",user?.UserPrincipalName }
                };
            _logger.LogInfo(LogMessages.GroupMonitorUserLicensingErrors, data);
        }
    }
}
