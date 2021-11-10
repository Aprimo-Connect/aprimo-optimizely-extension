using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public abstract class AprimoModelBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("parentId")]
        public object ParentId { get; set; }

        [JsonProperty("hasChildren")]
        public object HasChildren { get; set; }

        [JsonProperty("tag")]
        public object Tag { get; set; }

        [JsonProperty("modifiedOn")]
        public DateTimeOffset ModifiedOn { get; set; }

        [JsonProperty("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}