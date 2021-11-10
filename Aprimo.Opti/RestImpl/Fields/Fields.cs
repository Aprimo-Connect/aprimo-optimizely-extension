using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Opti.Models.RestImpl.Fields
{
    public partial class Field
    {
        public Field()
        {
            this.Links = new FieldLinks();
            this.Items = new List<FieldItem>();
        }

        [JsonProperty("_links")]
        public FieldLinks Links { get; set; }

        [JsonProperty("items")]
        public List<FieldItem> Items { get; set; }
    }
}