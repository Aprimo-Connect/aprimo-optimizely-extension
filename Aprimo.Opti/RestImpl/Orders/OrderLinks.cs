using Aprimo.Opti.Models.RestImpl;
using Newtonsoft.Json;

namespace Aprimo.Opti.RestImpl.Orders
{
    public partial class OrderLinks
    {
        [JsonProperty("targets")]
        public OrderCreatedBy Targets { get; set; }

        [JsonProperty("createdby")]
        public OrderCreatedBy Createdby { get; set; }

        [JsonProperty("self")]
        public Self Self { get; set; }
    }
}