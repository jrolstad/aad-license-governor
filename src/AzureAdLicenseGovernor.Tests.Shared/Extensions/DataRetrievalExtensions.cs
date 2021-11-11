using AzureAdLicenseGovernor.Core.Models.Data;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class DataRetrievalExtensions
    {
        public static DirectoryData GetDirectory(this TestBuilderBase testBuilder, string name)
        {
            return testBuilder.Context.Data
                .Directories
                .First(d => string.Equals(d.Value.Name, name, StringComparison.OrdinalIgnoreCase))
                .Value;

        }

        public static Microsoft.Graph.SubscribedSku GetProduct(this TestBuilderBase testBuilder, string skuPartNumber)
        {
            return testBuilder.Context.GraphApi
                .SubscribedSkus
                .First(d => string.Equals(d.Value.SkuPartNumber, skuPartNumber, StringComparison.OrdinalIgnoreCase))
                .Value;

        }

        public static Microsoft.Graph.Group GetGroup(this TestBuilderBase testBuilder, string name)
        {
            return testBuilder.Context.GraphApi
                .Groups
                .First(d => string.Equals(d.Value.DisplayName, name, StringComparison.OrdinalIgnoreCase))
                .Value;

        }

        public static LicensedGroupData GetGovernedGroup(this TestBuilderBase testBuilder, string name)
        {
            var group = testBuilder.GetGroup(name);

            var governedGroup = testBuilder.Context
                .Data
                .Groups
                .Where(g => g.Value.ObjectId == group.Id)
                .Select(g=>g.Value)
                .FirstOrDefault();

            return governedGroup;

        }

        public static IEnumerable<LogEntry> GetLogEntries(this TestBuilderBase testBuilder, string message)
        {
            return testBuilder.Context.Log.Logs
                .Where(l => l.Message == message);
        }
    }
}
