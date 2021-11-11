using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAdLicenseGovernor.Tests.Shared.TestContexts
{
    public class TestContext
    {
        public DataTestContext Data = new DataTestContext();
        public GraphApiContext GraphApi = new GraphApiContext();
        public AuthenticatedUserContext Identity = new AuthenticatedUserContext();
        public LoggingContext Log = new LoggingContext();
    }
}
