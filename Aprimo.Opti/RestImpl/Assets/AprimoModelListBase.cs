using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Opti.Models.RestImpl
{
    public abstract class AprimoModelListBase<T> where T : class
    {
        public AprimoModelListBase()
        {
            this.Items = new List<T>();
        }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("items")]
        public List<T> Items { get; set; }
    }
}