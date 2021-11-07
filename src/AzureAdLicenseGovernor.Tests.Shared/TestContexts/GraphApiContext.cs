using Microsoft.Graph;
using System.Collections.Concurrent;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class GraphApiContext
    {
        public ConcurrentDictionary<string, Group> Groups = new ConcurrentDictionary<string, Group>();
        public ConcurrentDictionary<string, SubscribedSku> SubscribedSkus = new ConcurrentDictionary<string, SubscribedSku>();
    }
}
