namespace AzureAdLicenseGovernor.Core.Models
{
    public class ServicePlan
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AppliesTo { get; set; }
        public string ProvisioningStatus { get; set; }
    }
}
