using AzureAdLicenseGovernor.Tests.Shared.Fakes;
using Microsoft.Azure.Functions.Worker;

namespace AzureAdLicenseGovernor.Tests.Shared.Extensions
{
    public static class TestBuilderExtensions
    {
        public static FunctionContext Context(this TestBuilderBase testBuilder)
        {
            return new FunctionContextFake(testBuilder.Context);
        }
    }
}
