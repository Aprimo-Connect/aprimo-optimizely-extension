using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.RestImpl.Orders
{
    public partial class Order
    {
        public Order()
        {
            this.DeliveredFiles = new List<string>();
            this.Links = new OrderLinks();
        }

        [JsonProperty("_links")]
        public OrderLinks Links { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("deliveredFiles")]
        public List<string> DeliveredFiles { get; set; }

        [JsonProperty("creatorEmail")]
        public string CreatorEmail { get; set; }

        [JsonProperty("disableNotification")]
        public bool DisableNotification { get; set; }

        [JsonProperty("earliestStartDate")]
        public object EarliestStartDate { get; set; }

        [JsonProperty("executionTime")]
        public DateTimeOffset ExecutionTime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("startedOn")]
        public object StartedOn { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        [JsonProperty("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}