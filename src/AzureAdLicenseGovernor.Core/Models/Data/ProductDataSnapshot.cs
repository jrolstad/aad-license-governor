using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzureAdLicenseGovernor.Core.Models.Data
{
    public class ProductDataSnapshot
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Area { get; set; }
        public DateTime SnapshotTakenAt { get; set; }
        public ICollection<ProductData> Products { get; set; }
    }
}
