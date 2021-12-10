using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public class Directory
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string GraphUrl { get; set; }
        public string PortalUrl { get; set; }
        public bool IsDefault { get; set; }
        public bool CanManageObjects { get; set; }
        public string ClientId { get; set; }
        public DirectoryClientType ClientType { get; set; }
        public DirectoryMonitoring Monitoring { get; set; }
    }

    public enum DirectoryClientType
    {
        Application,
        ManagedIdentity
    }

    public class DirectoryMonitoring
    {
        public bool TrackGroupLicenseAssignmentFailures { get; set; }
        public bool TrackUserLicenseAssignmentFailures { get; set; }
        public bool TrackProductChanges { get; set; }
        public bool TrackProductUsage { get; set; }
    }
}
