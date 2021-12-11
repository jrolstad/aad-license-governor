using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Core.Models
{
    public static class LogMessages
    {
        public const string GroupMonitorLicensingErrorEvent = "Group Monitor|Groups with licensing errors";
        public const string GroupMonitorUserLicensingErrorEvent = "Group Monitor|Users with licensing errors";
        public const string GroupMonitorLicensingErrors = "Group Monitor|Licensing Errors";
        public const string GroupMonitorUserLicensingErrors = "Group Monitor|User Licensing Errors";
        public const string GroupMonitorUserLicensingStates = "Group Monitor|User Licensing State";
        public const string GroupMonitorProcessingState = "Group Monitor|Processing State";

        public const string LicenseGovernanceAssignmentChange = "License Governance|Group License Assignment Change";
        public const string LicenseGovernanceChangeSummary = "License Governance|Change Summary";
        public const string LicenseGovernanceNoChange = "License Governance|Change Summary:None";

        public const string ProductMonitoringChangeSummary = "Product Monitoring|Change Summary";
        public const string ProductMonitoringNoChange = "Product Monitoring|Change Summary:None";
        public const string ProductMonitoringNewProduct = "Product Monitoring|New Product";
        public const string ProductMonitoringRemovedProduct = "Product Monitoring|Removed Product";
        public const string ProductMonitoringUpdatedProduct = "Product Monitoring|Updated Product";

        public const string ProductMonitoringNewServicePlan = "Product Monitoring|New Service Plan";
        public const string ProductMonitoringRemovedServicePlan = "Product Monitoring|Removed Service Plan";

        public const string ProductMonitoringLicenseUsage = "Product Monitoring|LicenseUsage";
    }
}
