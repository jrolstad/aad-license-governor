using AzureAdLicenseGovernor.Core.Models.Data;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class DataTestContext
    {
        public ConcurrentDictionary<string, DirectoryData> Directories = new ConcurrentDictionary<string, DirectoryData>();
        public ConcurrentDictionary<string, LicensedGroupData> Groups = new ConcurrentDictionary<string, LicensedGroupData>();
    }
}
