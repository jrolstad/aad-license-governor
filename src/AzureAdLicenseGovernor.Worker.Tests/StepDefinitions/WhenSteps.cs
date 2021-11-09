using AzureAdLicenseGovernor.Worker.Functions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace AzureAdLicenseGovernor.Worker.Tests.StepDefinitions
{
    [Binding]
    public class WhenSteps
    {
        private readonly TestBuilder _testBuilder;

        public WhenSteps(TestBuilder testBuilder)
        {
            _testBuilder = testBuilder;
        }

        [When(@"the license configuration is applied")]
        public async Task WhenTheLicenseConfigurationIsApplied()
        {
            var function = _testBuilder.Get<ApplyLicenseFunctions>();

            await function.Apply(new TimerInfo());
        }

    }
}
